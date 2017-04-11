using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Beestjes/Quest Definition")]
public class QuestDefinition : ScriptableObject
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

    public DateTime CalculateDeadline()
    {
        //Calculate the deadline
        DateTime currentDateTime = GameClock.Instance.GetDateTime();
        DateTime deadline = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day,
                                         (int)m_DeadlineTime.x, (int)m_DeadlineTime.y, (int)m_DeadlineTime.z);

        //Check if the deadline has already passed for today, set it to tomorrow
        if (currentDateTime > deadline)
        {
            deadline = deadline.AddDays(1);
        }

        return deadline;
    }
}
