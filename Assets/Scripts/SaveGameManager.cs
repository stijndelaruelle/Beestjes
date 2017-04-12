using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveGameManager : Singleton<SaveGameManager>
{
    public delegate void JSONClassDelegate(JSONClass jsonClass);
    public delegate void JSONNodeDelegate(JSONNode jsonNode);

    [Header("Root Path")]
    [Space(5)]
    [SerializeField]
    private string m_RootPath;
    private DirectoryInfo m_RootDirectory;

    [SerializeField]
    private string m_RootPicturePath;
    private DirectoryInfo m_RootPictureDirectory;

    [Header("Save Game")]
    [Space(5)]

    [SerializeField]
    private string m_WorldFileName;

    [SerializeField]
    private string m_InventoryFileName;

    [SerializeField]
    private string m_PictureAlbumFileName;

    [SerializeField]
    private string m_QuestManagerFileName;

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

    [SerializeField]
    private PictureAlbum m_PictureAlbum;

    [SerializeField]
    private QuestManager m_QuestManager;

    //Executes after all the other objects have been started
    private void Start()
    {
        DetermineRootPath();
        DeserializeAll();
    }

    private void Update()
    {
        //Debug commands
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SerializeAll();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            DeserializeAll();
        }
    }

    private void DetermineRootPath()
    {
        //No more Application.persistentDataPath as it will never return the SD card.

        #if UNITY_ANDROID && !UNITY_EDITOR  
            m_RootPath = DataPathPlugin.GetFirstWriteableExternalDataPath();
            m_RootPicturePath = DataPathPlugin.GetExternalStoragePublicDirectory();
            //int numberOfStorageDevices = DataPathPlugin.GetExternalDataPathCount();

            ////Backwards as we prefer an external SD card!
            //for (int i = (numberOfStorageDevices - 1); i >= 0; --i)
            //{
            //    if (DataPathPlugin.CanReadExternalDataPath(i) && DataPathPlugin.CanWriteExternalDataPath(i))
            //    {
            //        m_RootPath = DataPathPlugin.GetExternalDataPath(i);
            //        break;
            //    }
            //}
        #endif

        m_RootDirectory = new DirectoryInfo(m_RootPath);
        m_RootPictureDirectory = new DirectoryInfo(m_RootPicturePath);

        Debug.Log("Registered root path: " + m_RootPath);
        Debug.Log("Registered picture root path: " + m_RootPicturePath);
    }


    //Save Game Serialization
    public bool SerializeAll()
    {
        bool success = SerializeWorld();
        if (!success)
            return false;

        success = SerializeInventory();
        if (!success)
            return false;

        success = SerializePictureAlbum();
        if (!success)
            return false;

        success = SerializeQuestManager();
        if (!success)
            return false;

        return success;
    }

    public bool SerializeWorld()
    {
        return Serialize(m_World.Serialize, m_WorldFileName);
    }

    public bool SerializeInventory()
    {
        return Serialize(m_Inventory.Serialize, m_InventoryFileName);
    }

    public bool SerializePictureAlbum()
    {
        return Serialize(m_PictureAlbum.Serialize, m_PictureAlbumFileName);
    }

    public bool SerializeQuestManager()
    {
        return Serialize(m_QuestManager.Serialize, m_QuestManagerFileName);
    }

    private bool Serialize(JSONClassDelegate serializeFunction, string fileName)
    {
        //Create the root object
        JSONClass rootObject = new JSONClass();

        serializeFunction(rootObject);

        //Write the JSON data (.ToString in release as it saves a lot of data compard to ToJSON)
        string jsonStr = "";
        #if UNITY_ANDROID && !UNITY_EDITOR
            jsonStr = rootObject.ToString();
        #else
                jsonStr = rootObject.ToJSON(0);
        #endif

        string filePath = m_RootPath + Path.DirectorySeparatorChar + fileName;
        File.WriteAllText(filePath, jsonStr);

        Debug.Log("Save game succesfully saved!");
        return true;
    }


    public bool DeserializeAll()
    {
        bool success = DeserializeWorld();
        if (!success)
            return false;

        success = DeserializeInventory();
        if (!success)
            return false;

        success = DeserializePictureAlbum();
        if (!success)
            return false;

        success = DeserializeQuestManager();
        if (!success)
            return false;

        return success;
    }

    public bool DeserializeWorld()
    {
        return Deserialize(m_World.Deserialize, m_World.OnNewUser, m_WorldFileName);
    }

    public bool DeserializeInventory()
    {
        return Deserialize(m_Inventory.Deserialize, m_Inventory.OnNewUser, m_InventoryFileName);
    }

    public bool DeserializePictureAlbum()
    {
        return Deserialize(m_PictureAlbum.Deserialize, null, m_PictureAlbumFileName);
    }

    public bool DeserializeQuestManager()
    {
        return Deserialize(m_QuestManager.Deserialize, m_QuestManager.OnNewUser, m_QuestManagerFileName);
    }


    private bool Deserialize(JSONNodeDelegate deserializeFunction, Action newUserFunction, string fileName)
    {
        //Read the file
        string fileText = "";
        try
        {
            string filePath = m_RootPath + Path.DirectorySeparatorChar + fileName;
            fileText = File.ReadAllText(filePath);
        }
        catch (Exception e)
        {
            //The file was not found, but that shouldn't crash the game!
            Debug.LogWarning("Error deserializing savegame: " + e.Message);
        }

        //If the file is empty, it's probably a new account
        if (fileText == "")
        {
            if (newUserFunction != null)
                newUserFunction();

            return true;
        }

        //Else parse it!
        try
        {
            JSONNode rootNode = JSON.Parse(fileText);

            deserializeFunction(rootNode);

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
    public bool SerializePicture(Picture picture)
    {
        if (picture == null)
            return false;

        DirectoryInfo pictureDirectory = m_RootPictureDirectory;
        if (m_PictureFolder != "")
        {
            pictureDirectory = FindOrCreateDirectory(m_RootPictureDirectory, m_PictureFolder);
        }

        byte[] bytes = picture.Texture.EncodeToPNG();

        string fileName = string.Format(m_PictureFileName, GameClock.Instance.GetDateTime().ToString("dd-MM-yyyy_HH-mm-ss"));
        fileName = FindUniqueFileName(pictureDirectory, fileName);

        string path = pictureDirectory.FullName + Path.DirectorySeparatorChar + fileName;
        
        File.WriteAllBytes(path, bytes);

        picture.TextureFilePath = path;

        Debug.Log("Picture succesfully saved!");

        #if UNITY_ANDROID && !UNITY_EDITOR
            DataPathPlugin.RunMediaScanner(path);
        #endif

        return true;
    }

    public Texture2D DeserializePicture(string path)
    {
        if (DoesFileExist(path) == false)
            return null;

        Texture2D texture = new Texture2D(1, 1);
        byte[] byteArr = File.ReadAllBytes(path);
        texture.LoadImage(byteArr); //Will resize the texture's dimensions.

        return texture;
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

    private string FindUniqueFileName(DirectoryInfo rootDirectory, string originalFileName)
    {
        int dotIndex = originalFileName.LastIndexOf(".");
        string extention = originalFileName.Substring(dotIndex);
        originalFileName = originalFileName.Remove(dotIndex);

        string uniqueFileName = "";
        bool fileExists = true;
        int count = 0;
        while (fileExists == true || count >= 100) //Safety net
        {
            uniqueFileName = originalFileName;
            if (count > 0) uniqueFileName += " (" + (count + 1) + ")";
            uniqueFileName += extention;

            fileExists = DoesFileExist(rootDirectory.FullName + Path.DirectorySeparatorChar + uniqueFileName);
            ++count;
        }

        return uniqueFileName;
    }

    public bool DoesFileExist(string path)
    {
        return File.Exists(path);
    }

    public bool DeleteFile(string path)
    {
        try
        {
            File.Delete(path);

            #if UNITY_ANDROID && !UNITY_EDITOR
                DataPathPlugin.RunMediaScanner(path);
            #endif
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }
    }
}
