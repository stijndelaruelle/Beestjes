using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestListDisplayPanel : MonoBehaviour
{
    [SerializeField]
    private QuestManager m_QuestManager;

    [SerializeField]
    private PictureAlbumGridPanel m_PictureAlbumGridPanel;

    [SerializeField]
    private QuestDisplayPanel m_QuestPanelPrefab;
    private List<QuestDisplayPanel> m_QuestPanels;
    private QuestDisplayPanel m_SelectedPanel;

    [SerializeField]
    private RectTransform m_Content;

    private void Awake()
    {
        m_QuestPanels = new List<QuestDisplayPanel>();
    }

    private void OnEnable()
    {
        List<Quest> quests = m_QuestManager.Quests;

        if (quests == null)
            return;

        for (int i = 0; i < quests.Count; ++i)
        {
            if (i >= m_QuestPanels.Count)
            {
                AddQuestPanel(quests[i]);
            }
            else
            {
                m_QuestPanels[i].Initialize(quests[i]);
            }
        }

        if (m_QuestPanels.Count > quests.Count)
        {
            for (int i = m_QuestPanels.Count - 1; i >= quests.Count; --i)
            {
                RemoveQuestPanel(i);
            }
        }


        //Select the first image in the list
        if (m_QuestPanels.Count > 0)
        {
            m_QuestPanels[0].Select();
        }
    }

    private void OnDestroy()
    {
        foreach (QuestDisplayPanel questPanel in m_QuestPanels)
        {
            questPanel.QuestDisplayPanelSelectEvent -= OnQuestDisplayPanelSelect;
            GameObject.Destroy(questPanel.gameObject);
        }

        m_QuestPanels.Clear();
    }

    private void AddQuestPanel(Quest Quest)
    {
        QuestDisplayPanel newPanel = GameObject.Instantiate<QuestDisplayPanel>(m_QuestPanelPrefab, m_Content);
        newPanel.QuestDisplayPanelSelectEvent += OnQuestDisplayPanelSelect;

        newPanel.Initialize(Quest);

        m_QuestPanels.Add(newPanel);
    }

    private void RemoveQuestPanel(int id)
    {
        if (id < 0 || id >= m_QuestPanels.Count)
            return;

        m_QuestPanels[id].QuestDisplayPanelSelectEvent -= OnQuestDisplayPanelSelect;
        GameObject.Destroy(m_QuestPanels[id].gameObject);
        m_QuestPanels.RemoveAt(id);
    }

    //Events
    private void OnQuestDisplayPanelSelect(QuestDisplayPanel questDisplayPanel)
    {
        if (m_SelectedPanel != null)
        {
            m_SelectedPanel.Deselect();
        }

        m_SelectedPanel = questDisplayPanel;

        m_PictureAlbumGridPanel.AddFilter(questDisplayPanel.Quest.StartTime, questDisplayPanel.Quest.EndTime);
    }
}
