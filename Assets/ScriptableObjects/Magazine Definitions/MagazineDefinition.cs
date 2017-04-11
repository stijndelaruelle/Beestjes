using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Beestjes/Magazine Definition")]
public class MagazineDefinition : ScriptableObject
{
    [Space(5)]
    [SerializeField]
    private string m_Title;
    public string Title
    {
        get { return m_Title; }
    }

    [Space(5)]
    [SerializeField]
    private Vector3 m_DeadlineTime;
    public Vector3 DeadlineTime
    {
        get { return m_DeadlineTime; }
    }

    //These rewards will be dependant on score
    [SerializeField]
    private List<ItemDefinition> m_Rewards;
    public List<ItemDefinition> Rewards
    {
        get { return m_Rewards; }
    }
}
