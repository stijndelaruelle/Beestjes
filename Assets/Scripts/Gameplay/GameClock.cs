using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClock : Singleton<GameClock>
{
    [SerializeField]
    private bool m_CheatDateTime = false;

    [Tooltip ("Day/Month/Year")]
    [SerializeField]
    private Vector3 m_Date;

    [SerializeField]
    private Vector3 m_Time;
    private DateTime m_DebugDateTime;

    protected override void Awake()
    {
        base.Awake();
        Refresh();
    }

    //#if (UNITY_EDITOR)
    //private void Update()
    //{
    //    Refresh();
    //}
    //#endif
    
    private void Refresh()
    {
        m_DebugDateTime = new DateTime((int)m_Date.z, (int)m_Date.y, (int)m_Date.x, (int)m_Time.x, (int)m_Time.y, (int)m_Time.z);
    }

    public DateTime GetDateTime()
    {
        #if UNITY_EDITOR
            if (m_CheatDateTime)
                return m_DebugDateTime;
            else
                return DateTime.Now;
        #else
            return DateTime.Now;
        #endif
    }
}
