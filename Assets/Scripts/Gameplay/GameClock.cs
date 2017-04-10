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
    private DateTime m_CachedDateTime;

    //Mostly used for debugging
    public event BoolDelegate IsCheatingChangedEvent;
    public event DateTimeDelegate DateTimeChangedEvent;

    protected override void Awake()
    {
        base.Awake();

        m_DebugDateTime = new DateTime((int)m_InitialDebugDate.z, (int)m_InitialDebugDate.y, (int)m_InitialDebugDate.x,
                                       (int)m_InitialDebugTime.x, (int)m_InitialDebugTime.y, (int)m_InitialDebugTime.z);

        Refresh();
    }

    private void Refresh()
    {
        if (m_IsCheating) { m_CachedDateTime = m_DebugDateTime; }
        else              { m_CachedDateTime = DateTime.Now; }

        if (DateTimeChangedEvent != null)
            DateTimeChangedEvent(m_CachedDateTime);
    }

    public DateTime GetDateTime()
    {
        return m_CachedDateTime;
    }

    public void SetCheating(bool value)
    {
        m_IsCheating = value;

        if (IsCheatingChangedEvent != null)
            IsCheatingChangedEvent(m_IsCheating);

        Refresh();
    }


    public void AddDays(int days)
    {
        m_DebugDateTime = m_DebugDateTime.AddDays(days);
        Refresh();
    }

    public void AddMonths(int months)
    {
        m_DebugDateTime = m_DebugDateTime.AddMonths(months);
        Refresh();
    }

    public void AddYears(int years)
    {
        m_DebugDateTime = m_DebugDateTime.AddYears(years);
        Refresh();
    }

    public void AddHours(int hours)
    {
        m_DebugDateTime = m_DebugDateTime.AddHours(hours);
        Refresh();
    }

    public void AddMinutes(int minutes)
    {
        m_DebugDateTime = m_DebugDateTime.AddMinutes(minutes);
        Refresh();
    }

    public void AddSeconds(int seconds)
    {
        m_DebugDateTime = m_DebugDateTime.AddSeconds(seconds);
        Refresh();
    }
}
