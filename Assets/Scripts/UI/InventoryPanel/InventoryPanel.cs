using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class InventoryPanel : IVisible
{
    private RectTransform m_RectTransform;
    private Vector2 m_InitialPosition;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_InitialPosition = m_RectTransform.anchoredPosition;
    }

    public void Start()
    {
        Hide();
    }

    private void OnDestroy()
    {
        Hide();
    }

    public override void SetVisibility(bool value, bool fireEvent)
    {
        if (value == true)
        {
            m_Visuals.gameObject.SetActive(true);
            m_RectTransform.DOAnchorPosY(m_InitialPosition.y, 1).SetEase(Ease.OutBounce);
        }
        else
        {
            m_RectTransform.DOAnchorPosY(-m_RectTransform.rect.height, 0.5f).SetEase(Ease.OutCubic).OnComplete(OnSlideOutComplete);
        }

        if (fireEvent)
            FireVisibilityChangedEvent(value);
    }

    private void OnSlideOutComplete()
    {
        m_Visuals.gameObject.SetActive(false);
    }
}
