using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotator : MonoBehaviour
{
    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        //Use DateTime.Now but store the last used one somewhere on the device, stop updating the game if people put it back in time
        DateTime date = DateTime.Now;

        DateTime sunriseToday = DateTime.Now;
        DateTime sunsetToday = DateTime.Now;

        //Not used, remove from function later
        bool isSunrise = false;
        bool isSunset = false;

        SuntimeCalculator.CalculateSunRiseSetTimes(52.079208, 5.140324, DateTime.Now,
                                                   ref sunriseToday, ref sunsetToday, ref isSunrise, ref isSunset);

        //If it's day
        if (date > sunriseToday && date < sunsetToday)
        {
            double dayTimeUsed = (date - sunriseToday).TotalMilliseconds;
            double dayTime = (sunsetToday - sunriseToday).TotalMilliseconds;

            float dayTimeNormalized = (float)(dayTimeUsed / dayTime);
            float degrees = dayTimeNormalized * 180.0f * -1.0f;

            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, degrees);
        }

        //If it's night
        else
        {
            DateTime sunriseTomorrow = DateTime.Now;
            DateTime sunsetTomorrow = DateTime.Now;

            SuntimeCalculator.CalculateSunRiseSetTimes(52.079208, 5.140324, DateTime.Now.AddDays(1),
                                            ref sunriseTomorrow, ref sunsetTomorrow, ref isSunrise, ref isSunset);

            double nightTimeUsed = (date - sunsetToday).TotalMilliseconds;
            double nightTime = (sunriseTomorrow - sunsetToday).TotalMilliseconds;

            float dayTimeNormalized = (float)(nightTimeUsed / nightTime);
            float degrees = dayTimeNormalized * 180.0f * -1.0f;
            degrees += 180.0f;

            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, degrees);
        }
    }

}
