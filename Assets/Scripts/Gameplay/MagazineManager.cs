using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine
{
    private Picture m_Picture;
    public Picture Picture
    {
        get { return m_Picture; }
    }

    //Rewards (Definition?)

    public event PictureDelegate MagazinePictureChangedEvent;

    private void UpdateMagazine()
    {
        DateTime timestamp;
        if (m_Picture == null)
        {
            timestamp = GameClock.Instance.GetDateTime();
        }
        else
        {
            timestamp = m_Picture.TimeStamp;
        }

        //Determine when the next magazine deadline ends

        //Determine whether or not that time has been exceeded

        //If so, hand out rewards!

        //Once rewards are handed out delete this picture from the magazine (users can close the app while being prompted)
    }

    public void SetPicture(Picture picture)
    {
        if (m_Picture != null)
        {
            m_Picture.TextureChangedEvent -= OnPictureTextureChanged;
        }

        m_Picture = picture;
        m_Picture.TextureChangedEvent += OnPictureTextureChanged;

        if (MagazinePictureChangedEvent != null)
            MagazinePictureChangedEvent(picture);
    }

    public void Serialize(JSONClass rootNode)
    {
        //Picture
        JSONClass pictureNode = new JSONClass();
        m_Picture.Serialize(pictureNode);

        rootNode.Add("selected_picture", pictureNode);
    }

    public bool Deserialize(JSONClass rootNode)
    {
        JSONClass pictureNode = rootNode["selected_picture"].AsObject;

        Picture picture = new Picture();
        bool success = picture.Deserialize(pictureNode);

        if (success)
        {
            m_Picture = picture;  
        }

        UpdateMagazine();

        return success;
    }

    //Events
    private void OnPictureTextureChanged(Texture2D texture)
    {
        if (MagazinePictureChangedEvent != null)
            MagazinePictureChangedEvent(m_Picture);
    }
}

public class MagazineManager : MonoBehaviour
{
    public delegate void MagazineDelegate(Magazine magazine);
     
    private List<Magazine> m_Magazines;
    public List<Magazine> Magazines
    {
        get { return m_Magazines; }
    }

    public event MagazineDelegate MagazineChangedEvent;

    private void Awake()
    {
        m_Magazines = new List<Magazine>();
    }

    private void OnDestroy()
    {
        foreach (Magazine magazine in m_Magazines)
        {
            magazine.MagazinePictureChangedEvent -= OnMagazinePictureChangedEvent;
        }

        m_Magazines.Clear();
    }

    public void SetPicture(Picture picture)
    {
        //SUPER LAME, this entire setup is so that we can have multiple magazines running at once
        //But for now we only have one, change this later on

        m_Magazines[0].SetPicture(picture);
    }

    private void AddMagazine(Magazine magazine)
    {
        if (m_Magazines == null)
            return;

        magazine.MagazinePictureChangedEvent += OnMagazinePictureChangedEvent;
        m_Magazines.Add(magazine);
    }


    public void Serialize(JSONNode rootNode)
    {
        JSONArray magazineArrayNode = new JSONArray();
        foreach (Magazine magazine in m_Magazines)
        {
            JSONClass magazineNode = new JSONClass();
            magazine.Serialize(magazineNode);

            magazineArrayNode.Add(magazineNode);
        }

        rootNode.Add("magazines", magazineArrayNode);
    }

    public void Deserialize(JSONNode rootNode)
    {
        JSONArray pictureArrayNode = rootNode["magazines"].AsArray;
        if (pictureArrayNode != null)
        {
            for (int i = 0; i < pictureArrayNode.Count; ++i)
            {
                JSONClass magazineNode = pictureArrayNode[i].AsObject;

                Magazine magazine = new Magazine();
                bool success = magazine.Deserialize(magazineNode);

                if (success)
                    AddMagazine(magazine);
            }
        }
    }

    //Events
    private void OnMagazinePictureChangedEvent(Picture picture)
    {
        SaveGameManager.Instance.SerializeMagazineManager();
    }

    public void OnNewUser()
    {
        //TEMP
        AddMagazine(new Magazine());
    }
}
