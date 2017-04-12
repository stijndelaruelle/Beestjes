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

    private InventoryItem m_InventoryItem;
    public InventoryItem InventoryItem
    {
        get { return m_InventoryItem; }
    }

    public void Initialize(InventoryItem inventoryItem)
    {
        m_InventoryItem = inventoryItem;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        m_Image.sprite = m_InventoryItem.Picture;

        m_Amount.text = m_InventoryItem.Amount.ToString();
        m_Amount.enabled = (m_InventoryItem.Amount > 1);
    }

    public void SelectItem()
    {

    }
}
