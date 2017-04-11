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

    private DateTime m_LastTickTime = DateTime.MinValue;
    private List<WorldObject> m_WorldObjects;

    private void Awake()
    {
        m_LastTickTime = DateTime.MinValue;
        m_WorldObjects = new List<WorldObject>();
    }

    private void Start()
    {
        GameClock.Instance.DateTimeChangedEvent += OnDateTimeChanged;
        OnDateTimeChanged(GameClock.Instance.GetDateTime());
    }

    private void OnDestroy()
    {
        GameClock gameClock = GameClock.Instance;

        if (gameClock != null)
            gameClock.DateTimeChangedEvent -= OnDateTimeChanged;
    }

    private void TickLoop()
    {
        DateTime now = GameClock.Instance.GetDateTime();

        //If we haven't played yet
        if (m_LastTickTime == DateTime.MinValue)
        {
            Tick(now);
            SaveGameManager.Instance.SerializeWorld();
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

        m_LastTickTime = GameClock.Instance.GetDateTime();
        SaveGameManager.Instance.SerializeWorld();
    }

    private void Tick(DateTime dateTime)
    {
        float percentageOfPODPassed = 0.0f;
        PartOfDay partOfDay = SuntimeCalculator.GetPartOfDay(52.079208, 5.140324, dateTime, ref percentageOfPODPassed);

        //Loop backwards so removing is not an issue
        //foreach (WorldObject worldObject in m_WorldObjects)
        for (int i = m_WorldObjects.Count - 1; i >= 0; --i)
        {
            m_WorldObjects[i].Tick(dateTime, partOfDay, percentageOfPODPassed);
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

    public int CalculatePictureScore(List<string> tagList, Rect cameraRect)
    {
        int score = 0;
        foreach (WorldObject worldObject in m_WorldObjects)
        {
            score += worldObject.CalculatePictureScore(tagList, cameraRect);
        }

        return score;
    }

    public WorldObject SpawnWorldObject(string prefabName, Vector3 position)
    {
        WorldObject worldObjectPrefab = Resources.Load<WorldObject>(prefabName);
        return SpawnWorldObject(worldObjectPrefab, position);
    }

    public WorldObject SpawnWorldObject(WorldObject worldObjectPrefab, Vector3 position)
    {
        GameObject go = GameObject.Instantiate(worldObjectPrefab.gameObject, position, Quaternion.identity, transform) as GameObject;
        WorldObject worldObject = go.GetComponent<WorldObject>();

        if (worldObject == null)
            throw new System.Exception("Couldn't spawn world object: " + worldObjectPrefab.name);

        //Add him to our list
        worldObject.DestroyEvent += OnWorldObjectDestroyed;
        m_WorldObjects.Add(worldObject);

        return worldObject;
    }

    private void RemoveWorldObject(WorldObject worldObject)
    {
        if (m_WorldObjects == null)
            return;

        worldObject.DestroyEvent -= OnWorldObjectDestroyed;
        Destroy(worldObject.gameObject);

        m_WorldObjects.Remove(worldObject);
    }

    //Events
    private void OnDateTimeChanged(DateTime dateTime)
    {
        TickLoop();
    }

    private void OnWorldObjectDestroyed(WorldObject worldObject, WorldObject newWorldObjectPrefab)
    {
        Vector3 position = worldObject.transform.position;
        RemoveWorldObject(worldObject);

        if (newWorldObjectPrefab != null)
        {
            WorldObject newWorldObject = SpawnWorldObject(newWorldObjectPrefab, position);
            newWorldObject.Initialize(worldObject.EndTime); //Spawn at the time the old object is removed
        }
    }

    public void OnNewUser()
    {
        //TEMP
        WorldObject newWorldObject = SpawnWorldObject("Bush_2_FullSize", new Vector3(0.0f, -1.0f, 0.0f));
        newWorldObject.Initialize(GameClock.Instance.GetDateTime());
    }

    //Serialization
    public void Serialize(JSONNode rootNode)
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

    public void Deserialize(JSONNode rootNode)
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
                throw new System.Exception("The world save game has an invalid \"last_tick_time\" node! Expected DateTime. Source: " + lastRefreshTimeNode.ToString() + " Exception: " + e.Message);
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
                worldObject.Deserialize(worldObjectNode); //Not need to initiaze, deserialization does that for us.
            }
        }
    }
}
