using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManagerPanel : MonoBehaviour
{
    [SerializeField]
    private QuestManager m_QuestManager;

    [SerializeField]
    private QuestDisplayPanel m_QuestPanelPrefab;
    private List<QuestDisplayPanel> m_QuestPanels;

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
            for (int i = quests.Count; i < m_QuestPanels.Count; ++i)
            {
                RemoveQuestPanel(i);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (QuestDisplayPanel QuestPanel in m_QuestPanels)
        {
            GameObject.Destroy(QuestPanel.gameObject);
        }

        m_QuestPanels.Clear();
    }

    private void AddQuestPanel(Quest Quest)
    {
        QuestDisplayPanel newPanel = GameObject.Instantiate<QuestDisplayPanel>(m_QuestPanelPrefab, m_Content);
        newPanel.Initialize(Quest);

        m_QuestPanels.Add(newPanel);
    }

    private void RemoveQuestPanel(int id)
    {
        if (id < 0 || id >= m_QuestPanels.Count)
            return;

        GameObject.Destroy(m_QuestPanels[id].gameObject);
        m_QuestPanels.RemoveAt(id);
    }
}
