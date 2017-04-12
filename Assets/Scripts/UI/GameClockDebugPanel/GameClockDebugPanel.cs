using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class GameClockDebugPanel : MonoBehaviour
{
    [SerializeField]
    private bool m_IsVisible = false;

    private RectTransform m_RectTransform;
    
    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        ChangeVisibility(m_IsVisible);
    }

    public void ToggleVisibility()
    {
        m_IsVisible = !m_IsVisible;
        ChangeVisibility(m_IsVisible);
    }

    private void ChangeVisibility(bool value)
    {
        if (value)
        {
            m_RectTransform.DOAnchorPosY(0.0f, 1.0f).SetEase(Ease.OutBounce);
        }
        else
        {
            m_RectTransform.DOAnchorPosY(m_RectTransform.rect.height, 0.5f).SetEase(Ease.OutCubic); 
        }
    }
}
