using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Quest
{
    public delegate void QuestDelegate(Quest quest);

    private QuestListDefinition m_QuestListDefinition;
    private QuestDefinition m_QuestDefinition;
    public QuestDefinition QuestDefinition
    {
        get { return m_QuestDefinition; }
    }

    private DateTime m_Deadline;
    public DateTime Deadline
    {
        get { return m_Deadline; }
    }

    private Picture m_SelectedPicture;
    public Picture SelectedPicture
    {
        get { return m_SelectedPicture; }
    }

    public event QuestDelegate QuestCompleteEvent;
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

    public void CheckIfComplete(DateTime dateTime)
    {
        //Determine if the quest ended
        if (dateTime >= m_Deadline)
        {
            if (QuestCompleteEvent != null)
                QuestCompleteEvent(this);
        }
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
        questNode.Add("quest_id", new JSONData(id));

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
    public delegate void QuestDelegate(Quest quest);

    [SerializeField]
    private QuestListDefinition m_QuestListDefinition;

    private List<Quest> m_Quests;
    public List<Quest> Quests
    {
        get { return m_Quests; }
    }

    public event QuestDelegate QuestCompleteEvent;

    private void Awake()
    {
        m_Quests = new List<Quest>();
    }

    private void Start()
    {
        GameClock.Instance.DateTimeChangedEvent += OnDateTimeChanged;
    }

    private void OnDestroy()
    {
        GameClock gameClock = GameClock.Instance;

        if (gameClock != null)
            gameClock.DateTimeChangedEvent -= OnDateTimeChanged;

        foreach (Quest quest in m_Quests)
        {
            quest.QuestCompleteEvent -= OnQuestCompleted;
            quest.QuestSelectedPictureChangedEvent -= OnQuestPictureChanged;
        }

        m_Quests.Clear();
    }

    public void SetPicture(Picture picture)
    {
        //SUPER LAME, this entire setup is so that we can have multiple Quests running at once
        //But for now we only have one, change this later on

        m_Quests[0].SetPicture(picture);
    }

    private void AddQuest(Quest quest)
    {
        if (m_Quests == null)
            return;

        quest.QuestSelectedPictureChangedEvent += OnQuestPictureChanged;
        m_Quests.Add(quest);

        SaveGameManager.Instance.SerializeQuestManager();
    }

    public void UpdateQuests(DateTime dateTime)
    {
        foreach (Quest quest in m_Quests)
        {
            quest.CheckIfComplete(dateTime);
        }
    }

    public void Serialize(JSONNode rootNode)
    {
        JSONArray questArrayNode = new JSONArray();
        foreach (Quest quest in m_Quests)
        {
            JSONClass questNode = new JSONClass();
            quest.Serialize(questNode);

            questArrayNode.Add(questNode);
        }

        rootNode.Add("Quests", questArrayNode);
    }

    public void Deserialize(JSONNode rootNode)
    {
        JSONArray pictureArrayNode = rootNode["Quests"].AsArray;
        if (pictureArrayNode != null)
        {
            for (int i = 0; i < pictureArrayNode.Count; ++i)
            {
                JSONClass questNode = pictureArrayNode[i].AsObject;

                Quest quest = new Quest(m_QuestListDefinition);
                quest.QuestCompleteEvent += OnQuestCompleted;

                quest.Deserialize(questNode);
                AddQuest(quest);
            }
        }

        UpdateQuests(GameClock.Instance.GetDateTime());
    }

    //Events
    private void OnQuestCompleted(Quest quest)
    {
        //Passing trough
        if (QuestCompleteEvent != null)
            QuestCompleteEvent(quest);
    }

    private void OnQuestPictureChanged(Picture picture)
    {
        SaveGameManager.Instance.SerializeQuestManager();
    }

    private void OnDateTimeChanged(DateTime dateTime)
    {
        UpdateQuests(dateTime);
    }

    public void OnNewUser()
    {
        //TEMP
        QuestDefinition firstQuestDefinition = m_QuestListDefinition.GetQuestDefinition(0);
        Quest quest = new Quest(m_QuestListDefinition, firstQuestDefinition, firstQuestDefinition.CalculateDeadline());
        quest.QuestCompleteEvent += OnQuestCompleted;

        AddQuest(quest);
    }
}
