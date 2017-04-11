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
        List<Quest> Quests = m_QuestManager.Quests;

        if (Quests == null)
            return;

        for (int i = 0; i < Quests.Count; ++i)
        {
            if (i >= m_QuestPanels.Count)
            {
                AddQuestPanel(Quests[i]);
            }
            else
            {
                m_QuestPanels[i].Initialize(Quests[i]);
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
}
