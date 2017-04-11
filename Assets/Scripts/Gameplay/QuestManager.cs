using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    private Picture m_Picture;
    public Picture Picture
    {
        get { return m_Picture; }
    }

    //Rewards (Definition?)

    public event PictureDelegate QuestPictureChangedEvent;

    private void UpdateQuest()
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

        //Determine when the next Quest deadline ends

        //Determine whether or not that time has been exceeded

        //If so, hand out rewards!

        //Once rewards are handed out delete this picture from the Quest (users can close the app while being prompted)
    }

    public void SetPicture(Picture picture)
    {
        if (m_Picture != null)
        {
            m_Picture.TextureChangedEvent -= OnPictureTextureChanged;
        }

        m_Picture = picture;
        m_Picture.TextureChangedEvent += OnPictureTextureChanged;

        if (QuestPictureChangedEvent != null)
            QuestPictureChangedEvent(picture);
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

        UpdateQuest();

        return success;
    }

    //Events
    private void OnPictureTextureChanged(Texture2D texture)
    {
        if (QuestPictureChangedEvent != null)
            QuestPictureChangedEvent(m_Picture);
    }
}

public class QuestManager : MonoBehaviour
{
    public delegate void QuestDelegate(Quest Quest);
     
    private List<Quest> m_Quests;
    public List<Quest> Quests
    {
        get { return m_Quests; }
    }

    public event QuestDelegate QuestChangedEvent;

    private void Awake()
    {
        m_Quests = new List<Quest>();
    }

    private void OnDestroy()
    {
        foreach (Quest Quest in m_Quests)
        {
            Quest.QuestPictureChangedEvent -= OnQuestPictureChangedEvent;
        }

        m_Quests.Clear();
    }

    public void SetPicture(Picture picture)
    {
        //SUPER LAME, this entire setup is so that we can have multiple Quests running at once
        //But for now we only have one, change this later on

        m_Quests[0].SetPicture(picture);
    }

    private void AddQuest(Quest Quest)
    {
        if (m_Quests == null)
            return;

        Quest.QuestPictureChangedEvent += OnQuestPictureChangedEvent;
        m_Quests.Add(Quest);
    }


    public void Serialize(JSONNode rootNode)
    {
        JSONArray QuestArrayNode = new JSONArray();
        foreach (Quest Quest in m_Quests)
        {
            JSONClass QuestNode = new JSONClass();
            Quest.Serialize(QuestNode);

            QuestArrayNode.Add(QuestNode);
        }

        rootNode.Add("Quests", QuestArrayNode);
    }

    public void Deserialize(JSONNode rootNode)
    {
        JSONArray pictureArrayNode = rootNode["Quests"].AsArray;
        if (pictureArrayNode != null)
        {
            for (int i = 0; i < pictureArrayNode.Count; ++i)
            {
                JSONClass QuestNode = pictureArrayNode[i].AsObject;

                Quest Quest = new Quest();
                bool success = Quest.Deserialize(QuestNode);

                if (success)
                    AddQuest(Quest);
            }
        }
    }

    //Events
    private void OnQuestPictureChangedEvent(Picture picture)
    {
        SaveGameManager.Instance.SerializeQuestManager();
    }

    public void OnNewUser()
    {
        //TEMP
        AddQuest(new Quest());
    }
}
