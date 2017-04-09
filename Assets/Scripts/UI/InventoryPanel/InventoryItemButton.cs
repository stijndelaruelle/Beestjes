using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : MonoBehaviour
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

    private WorldObjectBuilderUI m_WorldObjectBuilder;

    public void Initialize(WorldObjectBuilderUI worldBuilder, InventoryItem inventoryItem)
    {
        m_WorldObjectBuilder = worldBuilder;
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
        m_WorldObjectBuilder.SelectItem(m_InventoryItem);
    }

}
