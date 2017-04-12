using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMoonRotator : MonoBehaviour
{
    public delegate void SunMoonDelegate();
    public event SunMoonDelegate RefreshEvent;

    private void Start()
    {
        GameClock.Instance.DateTimeChangedEvent += OnDateTimeChanged;
    }

    private void OnDestroy()
    {
        GameClock gameClock = GameClock.Instance;

        if (gameClock != null)
            gameClock.DateTimeChangedEvent -= OnDateTimeChanged;
    }

    private void Refresh(DateTime date)
    {
        float percentage = 0.0f;
        PartOfDay partOfDay = SuntimeCalculator.GetPartOfDay(52.079208, 5.140324, date, ref percentage);

        //If it's day
        float degrees = 0.0f;

        //TODO: Lame, quickly just for demo. This system will eventually change drastically
        switch (partOfDay)
        {
            case PartOfDay.Dawn:
                degrees = (percentage * 10.0f * -1.0f) + 5.0f;
                break;

            case PartOfDay.Day:
                degrees = (percentage * 170.0f * -1.0f) - 5.0f;
                break;

            case PartOfDay.Dusk:
                degrees = (percentage * 10.0f * -1.0f) + 185.0f;
                break;

            case PartOfDay.Night:
                degrees = (percentage * 170.0f * -1.0f) + 175.0f;
                break;

            default:
                degrees = 0.0f;
                break;
        }
        
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, degrees);

        if (RefreshEvent != null)
            RefreshEvent();
    }

    //Events
    private void OnDateTimeChanged(DateTime dateTime)
    {
        Refresh(dateTime);
    }
}
