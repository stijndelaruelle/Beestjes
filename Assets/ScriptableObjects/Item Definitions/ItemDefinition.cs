using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Beestjes/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Space(5)]
    [SerializeField]
    private Sprite m_Sprite;
    public Sprite Sprite
    {
        get { return m_Sprite; }
    }

    [SerializeField]
    private WorldObject m_WorldObjectPrefab;
    public WorldObject WorldObjectPrefab
    {
        get { return m_WorldObjectPrefab; }
    }

    [SerializeField]
    private int m_Price;
    public int Price
    {
        get { return m_Price; }
    }
}
