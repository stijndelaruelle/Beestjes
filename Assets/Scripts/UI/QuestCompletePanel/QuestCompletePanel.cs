using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCompletePanel : IVisible
{
    [SerializeField]
    private QuestManager m_QuestManager;
    private Quest m_Quest;

    [SerializeField]
    private Text m_Title;

    [SerializeField]
    private PictureDisplay m_PictureDisplay;

    [SerializeField]
    private QuestRewardPanel m_QuestRewardPanel;

    public void Start()
    {
        m_QuestManager.QuestCompleteEvent += OnQuestComplete;
        m_QuestManager.QuestRemovedEvent += OnQuestRemoved;
        Hide();
    }

    private void OnDestroy()
    {
        if (m_QuestManager != null)
        {
            m_QuestManager.QuestCompleteEvent -= OnQuestComplete;
            m_QuestManager.QuestRemovedEvent -= OnQuestRemoved;
        }
    }

    private void Refresh()
    {
        //Title
        m_Title.text = "Quest complete: " + m_Quest.QuestDefinition.Title;

        //Picture
        if (m_Quest.SelectedPicture == null) //Should never happen
            m_PictureDisplay.Initialize(null);
        else
            m_PictureDisplay.Initialize(m_Quest.SelectedPicture.Texture);

        //Reward panel
        m_QuestRewardPanel.Initialize(m_Quest);
    }

    //Events
    private void OnQuestComplete(Quest quest)
    {
        m_Quest = quest;
        Refresh();
        Show();
    }

    private void OnQuestRemoved(Quest quest)
    {
        if (quest == m_Quest)
        {
            Hide();
        }
    }
}
