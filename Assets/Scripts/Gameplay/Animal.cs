using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
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
}
