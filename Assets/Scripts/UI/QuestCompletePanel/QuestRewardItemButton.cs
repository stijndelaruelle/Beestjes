using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardItemButton : MonoBehaviour
{
    [SerializeField]
    private Image m_Image;

    [SerializeField]
    private Text m_Amount;

    private QuestRewardPanel m_QuestRewardPanel;

    private InventoryItem m_InventoryItem;
    public InventoryItem InventoryItem
    {
        get { return m_InventoryItem; }
    }

    public void Initialize(QuestRewardPanel questRewardPanel, InventoryItem inventoryItem)
    {
        m_QuestRewardPanel = questRewardPanel;
        m_InventoryItem = inventoryItem;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        m_Image.sprite = m_InventoryItem.Picture;

        m_Amount.text = m_InventoryItem.Amount.ToString();
        m_Amount.enabled = (m_InventoryItem.Amount > 1);
    }

    public void ClaimReward()
    {
        m_QuestRewardPanel.ClaimReward(m_InventoryItem);
    }
}
