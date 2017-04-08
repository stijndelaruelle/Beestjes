using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : MonoBehaviour
{
    [SerializeField]
    private Image m_Image;

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
        m_Image.sprite = inventoryItem.Picture;
    }

    public void SelectItem()
    {
        m_WorldObjectBuilder.SelectItem(m_InventoryItem);
    }

}
