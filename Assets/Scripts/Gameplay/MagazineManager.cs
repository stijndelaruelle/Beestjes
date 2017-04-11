﻿using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine
{

}

public class MagazineManager : MonoBehaviour
{
    private Picture m_Picture;
    public Picture Picture
    {
        get { return m_Picture; }
    }

    public event PictureDelegate MagazinePictureChangedEvent;

    //Rewards

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
        m_Picture = picture;

        if (MagazinePictureChangedEvent != null)
            MagazinePictureChangedEvent(picture);

        SaveGameManager.Instance.SerializeCompetition();
    }

    public void Serialize(JSONNode rootNode)
    {
        JSONClass pictureNode = new JSONClass();
        m_Picture.Serialize(pictureNode);

        rootNode.Add("picture", pictureNode);
    }

    public void Deserialize(JSONNode rootNode)
    {
        JSONClass pictureNode = rootNode["picture"].AsObject;

        Picture picture = new Picture();
        bool success = picture.Deserialize(pictureNode);

        if (success)
            m_Picture = picture;

        UpdateMagazine();
    }
}