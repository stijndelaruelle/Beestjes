using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Beestjes/Item List Definition")]
public class ItemListDefinition : ScriptableObject
{
    [SerializeField]
    private List<ItemDefinition> m_ItemList;

    //Accessors
    public ItemDefinition GetItemDefinition(int itemID)
    {
        if (m_ItemList == null)
            return null;

        if (itemID < 0 || itemID >= m_ItemList.Count)
            return null;

        return m_ItemList[itemID];
    }

    public int GetID(ItemDefinition itemDefinition)
    {
        if (m_ItemList == null)
            return -1;

        if (itemDefinition == null)
            return -1;

        return m_ItemList.IndexOf(itemDefinition);
    }

    public ItemDefinition GetItemDefinition(string worldObjectPrefabName)
    {
        foreach (ItemDefinition itemDefinition in m_ItemList)
        {
            if (itemDefinition.WorldObjectPrefab.name.ToLower() == worldObjectPrefabName.ToLower())
                return itemDefinition;
        }

        return null;
    }

    public int GetItemDefinitionCount()
    {
        if (m_ItemList == null)
            return -1;

        return m_ItemList.Count;
    }
}
