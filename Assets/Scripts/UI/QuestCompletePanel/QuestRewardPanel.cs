using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class QuestRewardPanel : MonoBehaviour
{
    [SerializeField]
    private ItemListDefinition m_ItemListDefinition;

    [SerializeField]
    private QuestRewardItemButton m_QuestRewardItemButtonPrefab;
    private List<QuestRewardItemButton> m_QuestRewardItemButtons;

    private Quest m_Quest;
    private RectTransform m_RectTransform;

    public void Initialize(Quest quest)
    {
        if (m_RectTransform == null)
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_QuestRewardItemButtons = new List<QuestRewardItemButton>();
        }

        m_Quest = quest;

        List<ItemDefinition> rewards = quest.QuestDefinition.Rewards;

        if (rewards == null)
            return;

        for (int i = 0; i < rewards.Count; ++i)
        {
            InventoryItem inventoryItem = new InventoryItem(m_ItemListDefinition, rewards[i], 1);
            if (i >= m_QuestRewardItemButtons.Count)
            {
                AddQuestRewardButton(inventoryItem);
            }
            else
            {
                m_QuestRewardItemButtons[i].Initialize(this, inventoryItem);
            }
        }
    }

    private void AddQuestRewardButton(InventoryItem inventoryItem)
    {
        QuestRewardItemButton newPanel = GameObject.Instantiate<QuestRewardItemButton>(m_QuestRewardItemButtonPrefab, m_RectTransform);
        newPanel.Initialize(this, inventoryItem);

        m_QuestRewardItemButtons.Add(newPanel);
    }

    public void ClaimReward(InventoryItem inventoryItem)
    {
        m_Quest.ClaimReward(inventoryItem);
    }
}
