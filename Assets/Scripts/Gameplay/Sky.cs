using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private Gradient m_DawnGradient;

    [SerializeField]
    private Gradient m_DayGradient;

    [SerializeField]
    private Gradient m_DuskGradient;

    [SerializeField]
    private Gradient m_NightGradient;

    private void Start()
    {
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
        DateTime date = GameClock.Instance.GetDateTime();

        float percentage = 0.0f;
        PartOfDay partOfDay = SuntimeCalculator.GetPartOfDay(52.079208, 5.140324, date, ref percentage);

        Gradient currentGradient;

        //TODO: Lame, quickly just for demo. This system will eventually change drastically
        switch (partOfDay)
        {
            case PartOfDay.Dawn:
                currentGradient = m_DawnGradient;
                break;

            case PartOfDay.Day:
                currentGradient = m_DayGradient;
                break;

            case PartOfDay.Dusk:
                currentGradient = m_DuskGradient;
                break;

            case PartOfDay.Night:
                currentGradient = m_NightGradient;
                break;

            default:
                currentGradient = m_DayGradient;
                break;
        }

        m_SpriteRenderer.color = currentGradient.Evaluate(percentage);
    }

}
