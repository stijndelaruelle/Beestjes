using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Quest
{
    private QuestListDefinition m_QuestListDefinition;
    private QuestDefinition m_QuestDefinition;

    private DateTime m_Deadline;

    private Picture m_SelectedPicture;
    public Picture SelectedPicture
    {
        get { return m_SelectedPicture; }
    }

    public event PictureDelegate QuestSelectedPictureChangedEvent;

    public Quest(QuestListDefinition questListDefinition)
    {
        m_QuestListDefinition = questListDefinition;
        m_QuestDefinition = null;
        m_Deadline = DateTime.MinValue;
    }

    public Quest(QuestListDefinition questListDefinition, QuestDefinition questDefinition, DateTime deadline)
    {
        m_QuestListDefinition = questListDefinition;
        m_QuestDefinition = questDefinition;
        m_Deadline = deadline;
    }

    private void UpdateQuest()
    {
        DateTime timestamp;
        if (m_SelectedPicture == null)
        {
            timestamp = GameClock.Instance.GetDateTime();
        }
        else
        {
            timestamp = m_SelectedPicture.TimeStamp;
        }

        //Determine when the next Quest deadline ends

        //Determine whether or not that time has been exceeded

        //If so, hand out rewards!

        //Once rewards are handed out delete this picture from the Quest (users can close the app while being prompted)
    }

    public void SetPicture(Picture picture)
    {
        if (m_SelectedPicture != null)
        {
            m_SelectedPicture.TextureChangedEvent -= OnPictureTextureChanged;
        }

        m_SelectedPicture = picture;
        m_SelectedPicture.TextureChangedEvent += OnPictureTextureChanged;

        if (QuestSelectedPictureChangedEvent != null)
            QuestSelectedPictureChangedEvent(picture);
    }

    public void Serialize(JSONClass questNode)
    {
        if (m_QuestDefinition == null)
            return;

        //Quest ID
        int id = m_QuestListDefinition.GetID(m_QuestDefinition);
        questNode.Add("item_id", new JSONData(id));

        //Deadline
        questNode.Add("deadline", m_Deadline.ToString("dd/MM/yyyy HH:mm:ss"));

        //Selected picture
        if (m_SelectedPicture != null)
        {
            JSONClass pictureNode = new JSONClass();
            m_SelectedPicture.Serialize(pictureNode);

            questNode.Add("selected_picture", pictureNode);
        }
    }

    public void Deserialize(JSONClass questNode)
    {
        //Quest ID
        int id = (questNode["quest_id"].AsInt);
        m_QuestDefinition = m_QuestListDefinition.GetQuestDefinition(id);

        //Deadline
        JSONNode timestampNode = questNode["deadline"];
        if (timestampNode != null)
        {
            try
            {
                m_Deadline = DateTime.ParseExact(timestampNode.Value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new System.Exception("The quest save file has an invalid \"deadline\" node! Expected DateTime. Source: " + timestampNode.ToString() + " Exception: " + e.Message);
            }
        }

        //Selected picture
        JSONClass pictureNode = questNode["selected_picture"].AsObject;

        if (pictureNode != null)
        {
            Picture picture = new Picture();
            bool success = picture.Deserialize(pictureNode);

            if (success)
            {
                m_SelectedPicture = picture;
            }
        }

        UpdateQuest();
    }

    //Events
    private void OnPictureTextureChanged(Texture2D texture)
    {
        if (QuestSelectedPictureChangedEvent != null)
            QuestSelectedPictureChangedEvent(m_SelectedPicture);
    }
}

public class QuestManager : MonoBehaviour
{
    public delegate void QuestDelegate(Quest Quest);

    [SerializeField]
    private QuestListDefinition m_QuestListDefinition;

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
            Quest.QuestSelectedPictureChangedEvent -= OnQuestPictureChangedEvent;
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

        Quest.QuestSelectedPictureChangedEvent += OnQuestPictureChangedEvent;
        m_Quests.Add(Quest);

        SaveGameManager.Instance.SerializeQuestManager();
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

                Quest Quest = new Quest(m_QuestListDefinition);
                Quest.Deserialize(QuestNode);
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
        QuestDefinition firstQuestDefinition = m_QuestListDefinition.GetQuestDefinition(0);
        AddQuest(new Quest(m_QuestListDefinition, firstQuestDefinition, firstQuestDefinition.CalculateDeadline()));
    }
}
