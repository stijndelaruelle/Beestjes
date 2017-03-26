using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Beestjes/Animal Definition")]
public class AnimalDefinition : ScriptableObject
{
    //Add date requirements later
    //[Header("Date")]
    //[Space(5)]
    //[SerializeField]
    //private DateTime m_StartTime;

    //Time requirements & chance

    //Weather requirements

    [Header("Appearance")]
    [Space(5)]
    [SerializeField]
    private Sprite m_Sprite;
    public Sprite Sprite
    {
        get { return m_Sprite; }
    }

    [Header ("Day")]
    [Space(5)]
    [MinMaxRange(0, 100)]
    [SerializeField]
    private MinMaxRange m_DayAppearanceTime;

    [Range(0, 100)]
    [SerializeField]
    private float m_DayAppearancePercentage;

    [Header("Night")]
    [Space(5)]
    [MinMaxRange(0, 100)]
    [SerializeField]
    private MinMaxRange m_NightAppearanceTime;

    [Range(0, 100)]
    [SerializeField]
    private float m_NightAppearancePercentage;

    [Header("Dawn")]
    [Space(5)]
    [MinMaxRange(0, 100)]
    [SerializeField]
    private MinMaxRange m_DawnAppearanceTime;

    [Range(0, 100)]
    [SerializeField]
    private float m_DawnAppearancePercentage;

    [Header("Dusk")]
    [Space(5)]
    [MinMaxRange(0, 100)]
    [SerializeField]
    private MinMaxRange m_DuskAppearanceTime;

    [Range(0, 100)]
    [SerializeField]
    private float m_DuskAppearancePercentage;

    private List<MinMaxRange> m_AppearanceTimes;
    private List<float> m_AppearancePercentages;

    private void Initialize()
    {
        //if (m_AppearanceTimes.Count > 0 && m_AppearancePercentages.Count > 0)
        //    return;

        //For easy access, fix this by writing a custom property drawer
        m_AppearanceTimes = new List<MinMaxRange>();
        m_AppearanceTimes.Add(m_DawnAppearanceTime);
        m_AppearanceTimes.Add(m_DayAppearanceTime);
        m_AppearanceTimes.Add(m_DuskAppearanceTime);
        m_AppearanceTimes.Add(m_NightAppearanceTime);

        m_AppearancePercentages = new List<float>();
        m_AppearancePercentages.Add(m_DawnAppearancePercentage);
        m_AppearancePercentages.Add(m_DayAppearancePercentage);
        m_AppearancePercentages.Add(m_DuskAppearancePercentage);
        m_AppearancePercentages.Add(m_NightAppearancePercentage);
    }

    public bool CanSpawn(PartOfDay partOfDay, float percentage)
    {
        Initialize();

        percentage *= 100.0f;

        int arrayID = (int)partOfDay;

        MinMaxRange appearanceTime = m_AppearanceTimes[arrayID];
        float appearancePercentage = m_AppearancePercentages[arrayID];

        //Check if we are within range
        bool isWithinRange = appearanceTime.IsWithinRange(percentage);
        if (isWithinRange == false)
            return false;

        //Check if we get trough the luck factor
        float rand = UnityEngine.Random.Range(0.0f, 10000.0f);
        rand /= 100.0f;

        return (rand <= appearancePercentage);
    }
}
