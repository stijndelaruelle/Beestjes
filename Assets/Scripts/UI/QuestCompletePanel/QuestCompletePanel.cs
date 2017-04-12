using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCompletePanel : MonoBehaviour
{
    [SerializeField]
    private QuestManager m_QuestManager;
    private Quest m_Quest;

    [SerializeField]
    private GameObject m_Visuals; //Workaround so this object can subscribe itself to the picture camera event.

    [SerializeField]
    private Text m_Title;

    [SerializeField]
    private PictureDisplay m_PictureDisplay;

    [SerializeField]
    private QuestRewardPanel m_QuestRewardPanel;

    public void Start()
    {
        m_QuestManager.QuestCompleteEvent += OnQuestComplete;
        Hide();
    }

    private void OnDestroy()
    {
        if (m_QuestManager != null)
            m_QuestManager.QuestCompleteEvent -= OnQuestComplete;
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

    private void OnQuestComplete(Quest quest)
    {
        m_Quest = quest;
        Refresh();
        Show();
    }

    public void Show()
    {
        m_Visuals.SetActive(true);
    }

    public void Hide()
    {
        m_Visuals.SetActive(false);
    }
}
