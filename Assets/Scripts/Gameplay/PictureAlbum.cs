using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Picture
{
    public delegate void Texture2DDelegate(Texture2D texture2D);

    private string m_TextureFilePath;
    public string TextureFilePath
    {
        get { return m_TextureFilePath; }
        set { m_TextureFilePath = value; }
    }

    private Texture2D m_Texture;
    public Texture2D Texture
    {
        get { return m_Texture; }
    }

    private DateTime m_Timestamp;
    public DateTime TimeStamp
    {
        get { return m_Timestamp; }
    }

    private int m_Score;
    public int Score
    {
        get { return m_Score; }
    }

    //Todo split up in different categorie (animals, plants, objects, etc...) so we can filter even more precicely later on
    //+ Fixes localisation issue.
    private List<string> m_Tags;
    public List<string> Tags
    {
        get { return m_Tags; }
    }

    //Add more data
    //Favorite
    //Award winner (place, time)

    public event Texture2DDelegate TextureChangedEvent;

    public Picture()
    {

        SetTexture(null);
        m_Score = 0;
        m_Tags = new List<string>();
        m_Timestamp = DateTime.Now;
    }

    public Picture(Texture2D texture, DateTime timestamp, int score, List<string> tags)
    {
        m_TextureFilePath = "";
        SetTexture(texture);
        m_Timestamp = timestamp;
        m_Score = score;
        m_Tags = tags;
    }

    public bool Dispose()
    {
        bool success = SaveGameManager.Instance.DeleteFile(m_TextureFilePath);

        if (success)
        {
            SetTexture(null);
            m_TextureFilePath = "";
            m_Timestamp = DateTime.MinValue;
            m_Score = 0;
            m_Tags = null;
        }

        return success;
    }

    private void SetTexture(Texture2D texture)
    {
        if (m_Texture == texture)
            return;

        m_Texture = texture;

        if (TextureChangedEvent != null)
            TextureChangedEvent(m_Texture);
    }

    public void Serialize(JSONClass rootNode)
    {
        if (m_TextureFilePath == "" || m_Texture == null)
            return;

        rootNode.Add("texture_path", new JSONData(m_TextureFilePath));
        rootNode.Add("timestamp", m_Timestamp.ToString("dd/MM/yyyy HH:mm:ss"));
        rootNode.Add("score", new JSONData(m_Score));

        if (m_Tags.Count > 0)
        {
            JSONArray tagArrayNode = new JSONArray();
            foreach (string tag in m_Tags)
            {
                tagArrayNode.Add(new JSONData(tag));
            }

            rootNode.Add("tags", tagArrayNode);
        }
    }

    public bool Deserialize(JSONClass rootNode)
    {
        m_TextureFilePath = rootNode["texture_path"].Value;
        SetTexture(SaveGameManager.Instance.DeserializePicture(m_TextureFilePath));

        //Texture did not exist / was invalid
        if (m_Texture == null)
            return false;

        //Timstamp
        JSONNode timestampNode = rootNode["timestamp"];
        if (timestampNode != null)
        {
            try
            {
                m_Timestamp = DateTime.ParseExact(timestampNode.Value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new System.Exception("The save game has an invalid \"last_refresh_time\" node! Expected DateTime. Source: " + timestampNode.ToString() + " Exception: " + e.Message);
            }
        }

        m_Score = rootNode["score"].AsInt;

        m_Tags.Clear();

        JSONArray tagArrayNode = rootNode["tags"].AsArray;
        if (tagArrayNode != null)
        {
            for (int i = 0; i < tagArrayNode.Count; ++i)
            {
                string tag = tagArrayNode[i].Value;
                m_Tags.Add(tag);
            }
        }

        return true;
    }
}

public class PictureAlbum : MonoBehaviour
{
    private List<Picture> m_Pictures;
    public List<Picture> Pictures
    {
        get { return m_Pictures; }
    }

    public event PictureDelegate PictureAddEvent;
    public event PictureDelegate PictureEditEvent;
    public event PictureDelegate PictureRemoveEvent;

    private void Awake()
    {
        m_Pictures = new List<Picture>();
    }

    public void AddPicture(Picture picture)
    {
        if (m_Pictures == null)
            return;

        m_Pictures.Add(picture);

        if (PictureAddEvent != null)
            PictureAddEvent(picture);

        SaveGameManager.Instance.SerializePictureAlbum();
    }

    public void EditPicture(Picture picture)
    {
        if (PictureEditEvent != null)
            PictureEditEvent(picture);

        SaveGameManager.Instance.SerializePictureAlbum();
    }

    public void RemovePicture(Picture picture)
    {
        if (m_Pictures == null)
            return;

        if (m_Pictures.Contains(picture) == false)
            return;

        bool success = picture.Dispose();
        if (success == false)
            return;

        m_Pictures.Remove(picture);

        if (PictureRemoveEvent != null)
            PictureRemoveEvent(picture);

        SaveGameManager.Instance.SerializePictureAlbum();
    }


    public void Serialize(JSONNode rootNode)
    {
        JSONArray pictureArrayNode = new JSONArray();
        foreach (Picture picture in m_Pictures)
        {
            JSONClass pictureNode = new JSONClass();
            picture.Serialize(pictureNode);

            pictureArrayNode.Add(pictureNode);
        }

        rootNode.Add("picture_album", pictureArrayNode);
    }

    public void Deserialize(JSONNode rootNode)
    {
        JSONArray pictureArrayNode = rootNode["picture_album"].AsArray;
        if (pictureArrayNode != null)
        {
            for (int i = 0; i < pictureArrayNode.Count; ++i)
            {
                JSONClass pictureNode = pictureArrayNode[i].AsObject;

                Picture picture = new Picture();
                bool success = picture.Deserialize(pictureNode);

                if (success)
                    AddPicture(picture);
            }
        }
    }
}
