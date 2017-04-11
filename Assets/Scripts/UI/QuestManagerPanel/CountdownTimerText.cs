using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimerText : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    private DateTime m_TargetTime;

    public void SetTargetTime(DateTime targetTime)
    {
        m_TargetTime = targetTime;
    }

    private void Update()
    {
        DateTime now = GameClock.Instance.GetDateTime();
        if (now > m_TargetTime)
        {
            m_Text.text = "Deadline passed!";
            return;
        }

        TimeSpan diff = m_TargetTime - now;

        string text = "";

        int hours = diff.Hours;
        int minutes = diff.Minutes;
        int seconds = diff.Seconds;

        if (hours > 0)
        {
            text += hours + " hour";
            if (hours > 1) { text += "s"; }

            if (minutes > 0) { text += ", "; }
            else if (seconds > 0) { text += " & "; }
        }


        if (minutes > 0)
        {
            text += minutes + " minute";
            if (minutes > 1) { text += "s"; }

            if (seconds > 0) { text += " & "; }
        }

        if (seconds > 0)
        {
            text += seconds + " second";
            if (seconds > 1) { text += "s"; }
        }

        text += " remaining";

        m_Text.text = text;
    }
}
