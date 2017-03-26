using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotator : MonoBehaviour
{
    private void Awake()
    {
        Refresh();
    }

    private void Refresh()
    {
        //Use DateTime.Now but store the last used one somewhere on the device, stop updating the game if people put it back in time
        DateTime date = DateTime.Now;

        float percentage = 0.0f;
        PartOfDay partOfDay = SuntimeCalculator.GetPartOfDay(52.079208, 5.140324, DateTime.Now, ref percentage);

        //If it's day
        float degrees = percentage * 180.0f * -1.0f;

        //If it's dusk/night/dawn
        if (partOfDay != PartOfDay.Day)
            degrees += 180.0f;

        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, degrees);
    }

}
