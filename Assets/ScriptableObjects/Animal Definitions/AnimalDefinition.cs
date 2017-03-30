﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Beestjes/Animal Definition")]
public class AnimalDefinition : ScriptableObject
{
    [Serializable]
    public class AppearanceTime
    {
        [SerializeField]
        private PartOfDay m_PartOfDay;
        public PartOfDay PartOfDay
        {
            get { return m_PartOfDay; }
        }

        [MinMaxRange(0, 100)]
        [SerializeField]
        private MinMaxRange m_PercentageOfPODRange;
        public MinMaxRange PercantageOfPODRange
        {
            get { return m_PercentageOfPODRange; }
        }

        [Range(0, 100)]
        [SerializeField]
        private float m_PercentageLuck;
        public float PercentageLuck
        {
            get { return m_PercentageLuck; }
        }

        [MinMaxRange(0, 3600)]
        [Tooltip("If the animal appears, how long does he stay? (in minutes)")]
        [SerializeField]
        private MinMaxRange m_StayTime;
        public MinMaxRange StayTime
        {
            get { return m_StayTime; }
        }
    }

    //Add date requirements later
    //[Header("Date")]
    //[Space(5)]
    //[SerializeField]
    //private DateTime m_StartTime;

    //Time requirements & chance

    //Weather requirements

    [Header("Visuals")]
    [Space(5)]
    [SerializeField]
    private Sprite m_Sprite;
    public Sprite Sprite
    {
        get { return m_Sprite; }
    }

    [Header("Appearance Times")]
    [Space(5)]
    [SerializeField]
    private List<AppearanceTime> m_AppearanceTimes;

    //Returns how long the animal will spawn for
    public float CanSpawn(PartOfDay partOfDay, float percentageOfDay)
    {
        percentageOfDay *= 100.0f;

        foreach(AppearanceTime appearanceTime in m_AppearanceTimes)
        {
            if (appearanceTime.PartOfDay == partOfDay)
            {
                //Check if we are within range
                bool isWithinRange = appearanceTime.PercantageOfPODRange.IsWithinRange(percentageOfDay);
                if (isWithinRange == false)
                    return 0.0f;

                //Check if we get trough the luck factor
                float rand = UnityEngine.Random.Range(0.0f, 10000.0f);
                rand /= 100.0f;

                if (rand > appearanceTime.PercentageLuck)
                    return 0.0f;

                //If we do, how long does he stay?
                float stayTime = appearanceTime.StayTime.GetRandomValue();
                return stayTime;
            }
        }

        return 0.0f;
    }
}
