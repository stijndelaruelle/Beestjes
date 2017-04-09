using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveGameManager : Singleton<SaveGameManager>
{
    [Header("Root Path")]
    [Space(5)]
    [SerializeField]
    private string m_RootPath;
    private DirectoryInfo m_RootDirectory;

    [Header("Save Game")]
    [Space(5)]

    [SerializeField]
    private string m_SaveFileName;

    [Header("Pictures")]
    [Space(5)]

    [SerializeField]
    private string m_PictureFolder;

    [SerializeField]
    private string m_PictureFileName;

    [Header("Serializable Objects")]
    [Space(5)]
    [SerializeField]
    private World m_World;

    [SerializeField]
    private Inventory m_Inventory;

    private void Start()
    {
        DetermineRootPath();
        Deserialize();
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
            Deserialize();
        }
    }

    private void DetermineRootPath()
    {
        //No more Application.persistentDataPath as it will never return the SD card.

        #if UNITY_ANDROID && !UNITY_EDITOR  
            int numberOfStorageDevices = DataPathPlugin.GetExternalDataPathCount();

            //Backwards as we prefer an external SD card!
            for (int i = (numberOfStorageDevices - 1); i >= 0; --i)
            {
                if (DataPathPlugin.CanReadExternalDataPath(i) && DataPathPlugin.CanWriteExternalDataPath(i))
                {
                    m_RootPath = DataPathPlugin.GetExternalDataPath(i);
                    break;
                }
            }
        #endif

        m_RootDirectory = new DirectoryInfo(m_RootPath);
        Debug.Log("Registered root path: " + m_RootPath);
    }

    //Save Game Serialization
    public bool Serialize()
    {
        //Create the root object
        JSONClass rootObject = new JSONClass();

        m_World.Serialize(rootObject);
        m_Inventory.Serialize(rootObject);

        //Write the JSON data (.ToString in release as it saves a lot of data compard to ToJSON)
        string jsonStr = "";
        #if UNITY_ANDROID && !UNITY_EDITOR
            jsonStr = rootObject.ToString();
        #else
                jsonStr = rootObject.ToJSON(0);
        #endif

        string filePath = m_RootPath + "/" + m_SaveFileName;
        File.WriteAllText(filePath, jsonStr);

        Debug.Log("Save game succesfully saved!");
        return true;
    }

    public bool Deserialize()
    {
        //Read the file
        string fileText = "";
        try
        {
            string filePath = m_RootPath + "/" + m_SaveFileName;
            fileText = File.ReadAllText(filePath);
        }
        catch (Exception e)
        {
            //The file was not found, but that shouldn't crash the game!
            Debug.LogWarning("Error deserializing savegame: " + e.Message);
        }

        //If the file is empty, it's probably a new account
        if (fileText == "") { return true; }

        //Else parse it!
        try
        {
            JSONNode rootNode = JSON.Parse(fileText);

            m_World.Deserialize(rootNode);
            m_Inventory.Deserialize(rootNode);

            Debug.Log("Savegame succesfully loaded!");
        }

        catch (Exception e)
        {
            Debug.LogError("Savegame encountered the following parsing error: " + e.Message);
            return false;
        }

        return true;
    }

    //Picture Serialization
    public string SavePictureToDisk(Texture2D texture)
    {
        if (texture == null)
            return "";

        byte[] bytes = texture.EncodeToPNG();

        DirectoryInfo pictureDirectory = FindOrCreateDirectory(m_RootDirectory, m_PictureFolder);
        string fileName = string.Format(m_PictureFileName, GameClock.Instance.GetDateTime().ToString("dd-MM-yyyy_HH-mm-ss"));
        string path = pictureDirectory + "/" + fileName;

        File.WriteAllBytes(path, bytes);

        return path;
    }

    //Utility
    private DirectoryInfo FindOrCreateDirectory(DirectoryInfo rootDirectory, string name)
    {
        //Check if that folder already exists
        DirectoryInfo[] subDirectories = rootDirectory.GetDirectories();
        DirectoryInfo ourDirectory = null;

        foreach (DirectoryInfo directory in subDirectories)
        {
            if (directory.Name == name)
            {
                ourDirectory = directory;
                break;
            }
        }

        //If not, create it.
        if (ourDirectory == null)
        {
            ourDirectory = rootDirectory.CreateSubdirectory(name);
        }

        return ourDirectory;
    }
}
