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

    [SerializeField]
    private string m_DebugSaveFilePath;

    [SerializeField]
    private string m_SaveFileName;

    private DateTime m_LastTickTime;
    private List<WorldObject> m_WorldObjects;

    private void Awake()
    {
        m_WorldObjects = new List<WorldObject>();
    }

    private void Start()
    {
        //Path is different on Android
        string filePath = m_DebugSaveFilePath + "/" + m_SaveFileName;
        //Serialize(filePath);
        Deserialize(filePath);
    }

    private void Tick()
    {
        //Calculate how much time has passed since we last closed the app
        //Cache this somewhere, this really only needs to be calculated once per gametick (15 min)
        float percentageOfPODPassed = 0.0f;
        PartOfDay partOfDay = SuntimeCalculator.GetPartOfDay(52.079208, 5.140324, DateTime.Now, ref percentageOfPODPassed);
    }

    public bool Serialize(string filePath)
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

        File.WriteAllText(filePath, jsonStr);

        Debug.Log("Save game succesfully saved!");
        return true;
    }

    private void SerializeJSON(JSONNode rootNode)
    {
        rootNode.Add("last_refresh_time", new JSONData(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
    }

    public bool Deserialize(string filePath)
    {
        //Read the file
        string fileText = "";
        try
        {
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
        m_WorldObjects.Clear();

        //Read the last refresh timestamp
        JSONNode lastRefreshTimeNode = rootNode["last_refresh_time"];
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
                GameObject go = GameObject.Instantiate(Resources.Load(prefabName)) as GameObject;
                WorldObject worldObject = go.GetComponent<WorldObject>();

                if (worldObject == null)
                    throw new System.Exception("Couldn't spawn world object: " + prefabName);

                worldObject.Deserialize(worldObjectNode);

                //Add him to our list
                m_WorldObjects.Add(worldObject);
            }
        }
    }
}
