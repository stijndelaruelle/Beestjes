using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InventoryItemDelegate(InventoryItem inventoryItem);

public class InventoryItem
{
    private ItemListDefinition m_ItemListDefinition;
    private ItemDefinition m_ItemDefinition;

    public Sprite Picture
    {
        get { return m_ItemDefinition.Sprite; }
    }
    public WorldObject WorldObjectPrefab
    {
        get { return m_ItemDefinition.WorldObjectPrefab; }
    }

    private int m_Amount;
    public int Amount
    {
        get { return m_Amount; }
    }

    public event InventoryItemDelegate UpdateEvent;
    public event InventoryItemDelegate DestroyEvent;

    public InventoryItem(ItemListDefinition itemListDefinition)
    {
        m_ItemListDefinition = itemListDefinition;
        m_ItemDefinition = null;
        m_Amount = 0;
    }

    public InventoryItem(ItemListDefinition itemListDefinition, ItemDefinition itemDefinition, int amount)
    {
        m_ItemListDefinition = itemListDefinition;
        m_ItemDefinition = itemDefinition;
        m_Amount = amount;
    }

    public bool CanUse()
    {
        return (m_Amount > 0);
    }

    public void Use()
    {
        if (m_Amount > 0)
        {
            m_Amount -= 1;

            if (m_Amount <= 0)
            {
                if (DestroyEvent != null)
                    DestroyEvent(this);
            }
            else
            {
                if (UpdateEvent != null)
                    UpdateEvent(this);
            }
        }
    }

    public void Serialize(JSONClass itemNode)
    {
        if (m_ItemDefinition == null)
            return;

        int id = m_ItemListDefinition.GetID(m_ItemDefinition);
        itemNode.Add("item_id", new JSONData(id));
        itemNode.Add("amount", new JSONData(m_Amount));
    }

    public void Deserialize(JSONClass itemNode)
    {
        int id = (itemNode["item_id"].AsInt);
        m_ItemDefinition = m_ItemListDefinition.GetItemDefinition(id);

        m_Amount = itemNode["amount"].AsInt;
    }
}

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private ItemListDefinition m_ItemList;

    private List<InventoryItem> m_Items;
    public List<InventoryItem> Items
    {
        get { return m_Items; }
    }

    public event InventoryItemDelegate ItemAddedEvent;
    public event InventoryItemDelegate ItemUpdatedEvent;
    public event InventoryItemDelegate ItemRemovedEvent;

    private void Awake()
    {
        m_Items = new List<InventoryItem>();
    }

    private void Update()
    {
        //Debug commands
        if (Input.GetKeyDown(KeyCode.F11))
        {
            AddItem(m_ItemList.GetItemDefinition(0), 1);
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            AddItem(m_ItemList.GetItemDefinition(1), 1);
        }
    }

    private void AddItem(ItemDefinition itemDefinition, int amount)
    {
        InventoryItem newItem = new InventoryItem(m_ItemList, itemDefinition, amount);
        AddItem(newItem);
    }

    private void AddItem(InventoryItem inventoryItem)
    {
        if (m_Items == null)
            return;

        //TODO: Doesn't check for duplicates yet.
        inventoryItem.UpdateEvent += OnInventoryItemUpdatedEvent;
        inventoryItem.DestroyEvent += OnInventoryItemDestroyed;

        m_Items.Add(inventoryItem);

        //Send trough
        if (ItemAddedEvent != null)
            ItemAddedEvent(inventoryItem);

        Debug.Log("Inventory: Added " + inventoryItem.Amount + " " + inventoryItem);
    }

    public void UseItem(InventoryItem item)
    {
        if (m_Items == null)
            return;

        if (m_Items.Contains(item))
            item.Use();
    }

    //Events
    private void OnInventoryItemUpdatedEvent(InventoryItem inventoryItem)
    {
        //Send trough
        if (ItemUpdatedEvent != null)
            ItemUpdatedEvent(inventoryItem);
    }

    private void OnInventoryItemDestroyed(InventoryItem inventoryItem)
    {
        if (m_Items == null)
            return;

        inventoryItem.UpdateEvent -= OnInventoryItemUpdatedEvent;
        inventoryItem.DestroyEvent -= OnInventoryItemDestroyed;
        m_Items.Remove(inventoryItem);

        //Send trough
        if (ItemRemovedEvent != null)
            ItemRemovedEvent(inventoryItem);
    }

    public void OnNewUser()
    {
        //TEMP
        AddItem(m_ItemList.GetItemDefinition(0), 1);
        AddItem(m_ItemList.GetItemDefinition(1), 1);
    }

    public void Serialize(JSONNode rootNode)
    {
        JSONArray itemArrayNode = new JSONArray();
        foreach (InventoryItem item in m_Items)
        {
            JSONClass itemNode = new JSONClass();
            item.Serialize(itemNode);

            itemArrayNode.Add(itemNode);
        }

        rootNode.Add("inventory", itemArrayNode);
    }

    public void Deserialize(JSONNode rootNode)
    {
        JSONArray itemArrayNode = rootNode["inventory"].AsArray;
        if (itemArrayNode != null)
        {
            for (int i = 0; i < itemArrayNode.Count; ++i)
            {
                JSONClass itemNode = itemArrayNode[i].AsObject;

                InventoryItem inventoryItem = new InventoryItem(m_ItemList);
                inventoryItem.Deserialize(itemNode);

                AddItem(inventoryItem);
            }
        }
    }
}
