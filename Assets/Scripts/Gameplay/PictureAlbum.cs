﻿using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture
{
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

    private int m_Score;
    public int Score
    {
        get { return m_Score; }
    }

    private List<string> m_Tags;
    public List<string> Tags
    {
        get { return m_Tags; }
    }

    //Add more data
    //Favorite
    //Award winner (place, time)

    public Picture()
    {
        m_Texture = null;
        m_Score = 0;
        m_Tags = new List<string>();
    }

    public Picture(Texture2D texture, int score, List<string> tags)
    {
        m_Texture = texture;
        m_Score = score;
        m_Tags = tags;
    }

    public void Serialize(JSONClass rootNode)
    {
        rootNode.Add("texture_path", new JSONData(m_TextureFilePath));
        rootNode.Add("score", new JSONData(m_Score));

        JSONArray tagArrayNode = new JSONArray();
        foreach (string tag in m_Tags)
        {
            tagArrayNode.Add(new JSONData(tag));
        }

        rootNode.Add("tags", tagArrayNode);
    }

    public void Deserialize(JSONClass rootNode)
    {
        m_TextureFilePath = rootNode["texture_path"].Value;
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

        //Load picture from disk
    }
}

public class PictureAlbum : MonoBehaviour
{
    private List<Picture> m_Pictures;

    private void Awake()
    {
        m_Pictures = new List<Picture>();
    }


    public void AddPicture(Picture picture)
    {
        if (m_Pictures == null)
            return;

        m_Pictures.Add(picture);
        SaveGameManager.Instance.SerializePictureAlbum();
    }

    public void EditPicture()
    {

    }

    public void RemovePicture(Picture picture)
    {
        if (m_Pictures == null)
            return;

        if (m_Pictures.Contains(picture) == false)
            return;

        m_Pictures.Remove(picture);
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
                picture.Deserialize(pictureNode);

                AddPicture(picture);
            }
        }
    }
}