using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(InputField))]
public class GameClockDebugInputField : MonoBehaviour
{
    private enum DateTimePart
    {
        Day,
        Month,
        Year,
        Hour,
        Minute,
        Second
    }

    [SerializeField]
    private DateTimePart m_DateTimePart;
    private int m_CurrentValue = 0;

    private InputField m_InputField;

    private void Awake()
    {
        m_InputField = GetComponent<InputField>();
    }

    private void Start()
    {
        GameClock gameClock = GameClock.Instance;

        if (gameClock != null)
        {
            gameClock.DateTimeChangedEvent += OnDateTimeChanged;
            Refresh(gameClock.GetDateTime());
        }
    }

    private void OnDestroy()
    {
        GameClock gameClock = GameClock.Instance;

        if (gameClock != null)
            gameClock.DateTimeChangedEvent -= OnDateTimeChanged;
    }

    private void Refresh(DateTime dateTime)
    {
        switch (m_DateTimePart)
        {
            case DateTimePart.Day:    m_CurrentValue = dateTime.Day;    break;
            case DateTimePart.Month:  m_CurrentValue = dateTime.Month;  break;
            case DateTimePart.Year:   m_CurrentValue = dateTime.Year;   break;
            case DateTimePart.Hour:   m_CurrentValue = dateTime.Hour;   break;
            case DateTimePart.Minute: m_CurrentValue = dateTime.Minute; break;
            case DateTimePart.Second: m_CurrentValue = dateTime.Second; break;

            default:
                break;
        }

        m_InputField.text = m_CurrentValue.ToString();
    }

    public void Add(int amount)
    {
        GameClock gameClock = GameClock.Instance;

        switch (m_DateTimePart)
        {
            case DateTimePart.Day:    gameClock.AddDays(amount);    break;
            case DateTimePart.Month:  gameClock.AddMonths(amount);  break;
            case DateTimePart.Year:   gameClock.AddYears(amount);   break;
            case DateTimePart.Hour:   gameClock.AddHours(amount);   break;
            case DateTimePart.Minute: gameClock.AddMinutes(amount); break;
            case DateTimePart.Second: gameClock.AddSeconds(amount); break;

            default:
                break;
        }
    }

    //Events
    private void OnDateTimeChanged(DateTime dateTime)
    {
        Refresh(dateTime);
    }

    public void OnEndEdit(string value)
    {
        int i = -1;
        int.TryParse(value, out i);

        if (i == -1)
        {
            m_InputField.text = "0";
            return;
        }

        int diff = i - m_CurrentValue;
        Add(diff);
    }
}
