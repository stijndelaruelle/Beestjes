using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class World : MonoBehaviour
{
    public delegate void WorldTickDelegate(PartOfDay partOfDay, float percentageOfPODPassed);

    [Tooltip("Time between ticks (in minutes)")]
    [SerializeField]
    private int m_TimeBetweenTicks;

    [Tooltip("Max ticks executed at the same time")]
    [SerializeField]
    private int m_MaxTicks;

    [SerializeField]
    private string m_DebugSaveFilePath;

    [SerializeField]
    private string m_SaveFileName;

    private DateTime m_LastTickTime = DateTime.MinValue;
    private List<WorldObject> m_WorldObjects;

    private void Awake()
    {
        m_LastTickTime = DateTime.MinValue;
        m_WorldObjects = new List<WorldObject>();
    }

    private void Start()
    {
        Deserialize();
        TickLoop();
    }

    private void Update()
    {
        //Debug commands
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Serialize();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            string filePath = m_DebugSaveFilePath + "/" + m_SaveFileName;
            Deserialize();
        }
    }

    private void TickLoop()
    {
        DateTime now = GameClock.Instance.GetDateTime();

        //If we haven't played yet
        if (m_LastTickTime == DateTime.MinValue)
        {
            Tick(now);
            Serialize();
            return;
        }

        //Calculate the time between now and the last time we ticked
        TimeSpan timeSpan = now - m_LastTickTime;

        //Calculate how many ticks that is
        int tickCount = Mathf.FloorToInt((float)timeSpan.TotalMinutes) / m_TimeBetweenTicks;
        DateTime currentDateTime = m_LastTickTime;

        if (tickCount >= m_MaxTicks)
        {
            int diff = tickCount - m_MaxTicks;
            currentDateTime = currentDateTime.AddMinutes(diff * m_TimeBetweenTicks);

            tickCount = m_MaxTicks;
        }

        if (tickCount <= 0)
        {
            //Check if there are any animals at all, do 1 tick to up the chances
            foreach (WorldObject worldObject in m_WorldObjects)
            {
                bool hasAnimal = worldObject.HasAnimal();
                if (hasAnimal == true)
                    return;
            }

            tickCount = 1;
        }

        for (int i = 0; i < tickCount; ++i)
        {
            currentDateTime = currentDateTime.AddMinutes(m_TimeBetweenTicks);
            Tick(currentDateTime);
        }

        Serialize();
    }

    private void Tick(DateTime dateTime)
    {
        float percentageOfPODPassed = 0.0f;
        PartOfDay partOfDay = SuntimeCalculator.GetPartOfDay(52.079208, 5.140324, dateTime, ref percentageOfPODPassed);

        foreach (WorldObject worldObject in m_WorldObjects)
        {
            worldObject.Tick(dateTime, partOfDay, percentageOfPODPassed);
        }

        m_LastTickTime = dateTime;
    }

    private void ClearWorld()
    {
        foreach (WorldObject worldObject in m_WorldObjects)
        {
            worldObject.DestroyEvent -= OnWorldObjectDestroyed;
            Destroy(worldObject.gameObject);
        }

        m_WorldObjects.Clear();
    }

    public WorldObject SpawnWorldObject(string prefabName, Vector3 position)
    {
        GameObject go = GameObject.Instantiate(Resources.Load(prefabName), position, Quaternion.identity) as GameObject;
        WorldObject worldObject = go.GetComponent<WorldObject>();

        if (worldObject == null)
            throw new System.Exception("Couldn't spawn world object: " + prefabName);

        //Add him to our list
        worldObject.DestroyEvent += OnWorldObjectDestroyed;
        m_WorldObjects.Add(worldObject);

        return worldObject;
    }

    //Events
    private void OnWorldObjectDestroyed(WorldObject worldObject)
    {
        if (m_WorldObjects == null)
            return;

        worldObject.DestroyEvent -= OnWorldObjectDestroyed;
        Destroy(worldObject.gameObject);

        m_WorldObjects.Remove(worldObject);
    }

    //Serialization
    public bool Serialize()
    {
        //Create the root object
        JSONClass rootObject = new JSONClass();

        SerializeJSON(rootObject);

        //Write the JSON data (.ToString in release as it saves a lot of data compard to ToJSON)
        string jsonStr = "";
        #if UNITY_ANDROID && !UNITY_EDITOR
            jsonStr = rootObject.ToString();
        #else
                jsonStr = rootObject.ToJSON(0);
        #endif

        string filePath = m_DebugSaveFilePath + "/" + m_SaveFileName;
        File.WriteAllText(filePath, jsonStr);

        Debug.Log("Save game succesfully saved!");
        return true;
    }

    private void SerializeJSON(JSONNode rootNode)
    {
        rootNode.Add("last_tick_time", new JSONData(m_LastTickTime.ToString("dd/MM/yyyy HH:mm:ss")));

        JSONArray worldObjectArrayNode = new JSONArray();
        foreach (WorldObject worldObject in m_WorldObjects)
        {
            JSONClass worldObjectNode = new JSONClass();
            worldObject.Serialize(worldObjectNode);

            worldObjectArrayNode.Add(worldObjectNode);
        }

        rootNode.Add("world_objects", worldObjectArrayNode);
    }

    public bool Deserialize()
    {
        //Read the file
        string fileText = "";
        try
        {
            string filePath = m_DebugSaveFilePath + "/" + m_SaveFileName;
            fileText = File.ReadAllText(filePath);
        }
        catch (Exception e)
        {
            //The file was not found, but that shouldn't crash the game!
            Debug.LogWarning("Error deserializing savegame: " + e.Message);
        }

        //If the file is empty, it's probably a new account
        if (fileText == "") { return true; }

        try
        {
            JSONNode rootNode = JSON.Parse(fileText);
            DeserializeJSON(rootNode);

            Debug.Log("Savegame succesfully loaded!");
        }

        catch (Exception e)
        {
            Debug.LogError("Savegame encountered the following parsing error: " + e.Message);
            return false;
        }

        return true;
    }

    private void DeserializeJSON(JSONNode rootNode)
    {
        ClearWorld();

        //Read the last refresh timestamp
        JSONNode lastRefreshTimeNode = rootNode["last_tick_time"];
        if (lastRefreshTimeNode != null)
        {
            try
            {
                m_LastTickTime = DateTime.ParseExact(lastRefreshTimeNode.Value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new System.Exception("The save game has an invalid \"last_refresh_time\" node! Expected DateTime. Source: " + lastRefreshTimeNode.ToString() + " Exception: " + e.Message);
            }
        }

        //Read all the world objects
        JSONArray worldObjectsNode = rootNode["world_objects"].AsArray;
        if (worldObjectsNode != null)
        {
            for (int i = 0; i < worldObjectsNode.Count; ++i)
            {
                JSONClass worldObjectNode = worldObjectsNode[i].AsObject;

                string prefabName = worldObjectNode["prefab_name"].Value;

                //Spawn the prefab
                WorldObject worldObject = SpawnWorldObject(prefabName, Vector3.zero);
                worldObject.Deserialize(worldObjectNode);
            }
        }
    }

}
