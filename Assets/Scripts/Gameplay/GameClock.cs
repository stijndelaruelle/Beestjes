using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClock : Singleton<GameClock>
{
    public delegate void BoolDelegate(bool b);
    public delegate void DateTimeDelegate(DateTime dateTime);
     
    [SerializeField]
    private bool m_IsCheating = false;
    public bool IsCheating
    {
        get { return m_IsCheating; }
    }

    [Tooltip ("Day/Month/Year")]
    [SerializeField]
    private Vector3 m_InitialDebugDate;

    [SerializeField]
    private Vector3 m_InitialDebugTime;

    private DateTime m_DebugDateTime;

    //Mostly used for debugging
    public event BoolDelegate IsCheatingChangedEvent;
    public event DateTimeDelegate DateTimeChangedEvent;

    protected void Start()
    {
        m_DebugDateTime = new DateTime((int)m_InitialDebugDate.z, (int)m_InitialDebugDate.y, (int)m_InitialDebugDate.x,
                                       (int)m_InitialDebugTime.x, (int)m_InitialDebugTime.y, (int)m_InitialDebugTime.z);

        SendUpdateEvent();
    }

    private void SendUpdateEvent()
    {
        if (DateTimeChangedEvent != null)
            DateTimeChangedEvent(m_DebugDateTime);
    }

    public DateTime GetDateTime()
    {
        if (m_IsCheating) { return m_DebugDateTime; }
        else              { return DateTime.Now; }
    }

    public void SetCheating(bool value)
    {
        m_IsCheating = value;

        if (IsCheatingChangedEvent != null)
            IsCheatingChangedEvent(m_IsCheating);

        SendUpdateEvent();
    }


    public void AddDays(int days)
    {
        m_DebugDateTime = m_DebugDateTime.AddDays(days);
        SendUpdateEvent();
    }

    public void AddMonths(int months)
    {
        m_DebugDateTime = m_DebugDateTime.AddMonths(months);
        SendUpdateEvent();
    }

    public void AddYears(int years)
    {
        m_DebugDateTime = m_DebugDateTime.AddYears(years);
        SendUpdateEvent();
    }

    public void AddHours(int hours)
    {
        m_DebugDateTime = m_DebugDateTime.AddHours(hours);
        SendUpdateEvent();
    }

    public void AddMinutes(int minutes)
    {
        m_DebugDateTime = m_DebugDateTime.AddMinutes(minutes);
        SendUpdateEvent();
    }

    public void AddSeconds(int seconds)
    {
        m_DebugDateTime = m_DebugDateTime.AddSeconds(seconds);
        SendUpdateEvent();
    }
}
