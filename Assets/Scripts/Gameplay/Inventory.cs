using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void InventoryItemDelegate(InventoryItem inventoryItem);

public class InventoryItem
{
    private WorldObject m_WorldObjectPrefab;
    public WorldObject WorldObjectPrefab
    {
        get { return m_WorldObjectPrefab; }
    }

    private Sprite m_Picture;
    public Sprite Picture
    {
        get { return m_Picture; }
    }

    private int m_Amount;
    public int Amount
    {
        get { return m_Amount; }
    }

    public event InventoryItemDelegate DestroyEvent;

    public InventoryItem()
    {
        m_WorldObjectPrefab = null;
        m_Amount = 0;
    }

    public InventoryItem(string worldObjectPrefab, int amount)
    {
        SetPrefab(worldObjectPrefab);
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
        }
    }

    private void SetPrefab(string worldObjectPrefab)
    {
        m_WorldObjectPrefab = Resources.Load<WorldObject>(worldObjectPrefab);

        SpriteRenderer spriteRenderer = m_WorldObjectPrefab.GetComponentInChildren<SpriteRenderer>();
        m_Picture = spriteRenderer.sprite;
    }

    public void Serialize(JSONClass itemNode)
    {
        itemNode.Add("prefab_name", new JSONData(m_WorldObjectPrefab.name));
        itemNode.Add("amount", new JSONData(m_Amount));
    }

    public void Deserialize(JSONClass itemNode)
    {
        SetPrefab(itemNode["prefab_name"].Value);
        m_Amount = itemNode["amount"].AsInt;
    }
}

public class Inventory : MonoBehaviour
{
    private List<InventoryItem> m_Items;
    public List<InventoryItem> Items
    {
        get { return m_Items; }
    }

    public event InventoryItemDelegate ItemAddedEvent;
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
            AddItem("Bush_1_Small", 1);
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            AddItem("Tree_1_Small", 1);
        }
    }

    private void AddItem(string name, int amount)
    {
        InventoryItem newItem = new InventoryItem(name, amount);
        AddItem(newItem);
    }

    private void AddItem(InventoryItem inventoryItem)
    {
        if (m_Items == null)
            return;

        //TODO: Doesn't check for duplicates yet.
        inventoryItem.DestroyEvent += OnInventoryItemDestroyed;

        m_Items.Add(inventoryItem);

        //Send trough
        if (ItemAddedEvent != null)
            ItemAddedEvent(inventoryItem);

        Debug.Log("Inventory: Added " + inventoryItem.Amount + " " + inventoryItem.WorldObjectPrefab.name);
    }

    public void UseItem(InventoryItem item)
    {
        if (m_Items == null)
            return;

        if (m_Items.Contains(item))
            item.Use();
    }

    //Events
    private void OnInventoryItemDestroyed(InventoryItem inventoryItem)
    {
        if (m_Items == null)
            return;

        inventoryItem.DestroyEvent -= OnInventoryItemDestroyed;
        m_Items.Remove(inventoryItem);

        //Send trough
        if (ItemRemovedEvent != null)
            ItemRemovedEvent(inventoryItem);
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

                InventoryItem inventoryItem = new InventoryItem();
                inventoryItem.Deserialize(itemNode);

                AddItem(inventoryItem);
            }
        }
    }
}
