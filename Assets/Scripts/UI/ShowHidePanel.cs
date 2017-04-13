using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ShowHidePanel : MonoBehaviour
{
    private enum Side
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [SerializeField]
    private List<IVisible> m_Panels;

    [SerializeField]
    private bool m_ShowWhenPanelIsShown = false;

    [SerializeField]
    private bool m_HideOnAwake = true;

    [SerializeField]
    private Side m_Direction = Side.Top;

    private RectTransform m_RectTransform;
    private Vector2 m_InitialPosition;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_InitialPosition = m_RectTransform.anchoredPosition;

        if (m_HideOnAwake)
            ChangeVisibility(false, true);
    }

    private void Start()
    {
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        foreach (IVisible visiblePanel in m_Panels)
        {
            visiblePanel.VisibilityChangedEvent += OnPanelVisibilityChanged;
        }
    }

    private void OnDestroy()
    {
        foreach (IVisible visiblePanel in m_Panels)
        {
            visiblePanel.VisibilityChangedEvent -= OnPanelVisibilityChanged;
        }
    }

    private void OnPanelVisibilityChanged(bool value)
    {
        if (!m_ShowWhenPanelIsShown)
            value = !value;

        ChangeVisibility(value, false);
    }

    private void ChangeVisibility(bool show, bool instant)
    {
        m_RectTransform.DOComplete(false);

        if (show)
        {
            if (m_Direction == Side.Top || m_Direction == Side.Bottom)
            {
                float newPosition = m_InitialPosition.y;

                if (instant)
                    m_RectTransform.anchoredPosition = new Vector2(m_RectTransform.anchoredPosition.x, newPosition);
                else
                    m_RectTransform.DOAnchorPosY(newPosition, 1).SetEase(Ease.OutBounce);
            }

            if (m_Direction == Side.Left || m_Direction == Side.Right)
            {
                float newPosition = m_InitialPosition.x;

                if (instant)
                    m_RectTransform.anchoredPosition = new Vector2(newPosition, m_RectTransform.anchoredPosition.y);
                else
                    m_RectTransform.DOAnchorPosX(newPosition, 1).SetEase(Ease.OutBounce);
            }
        }
        else
        {
            if (m_Direction == Side.Top || m_Direction == Side.Bottom)
            {
                float newPosition = m_RectTransform.rect.height;

                if (m_Direction == Side.Bottom)
                    newPosition *= -1.0f;

                if (instant)
                    m_RectTransform.anchoredPosition = new Vector2(m_RectTransform.anchoredPosition.x, newPosition);
                else
                    m_RectTransform.DOAnchorPosY(newPosition, 0.5f).SetEase(Ease.OutCubic);

            }

            if (m_Direction == Side.Left || m_Direction == Side.Right)
            {
                float newPosition = m_RectTransform.rect.width;

                if (m_Direction == Side.Left)
                    newPosition *= -1.0f;

                if (instant)
                    m_RectTransform.anchoredPosition = new Vector2(newPosition, m_RectTransform.anchoredPosition.y);
                else
                    m_RectTransform.DOAnchorPosX(newPosition, 0.5f).SetEase(Ease.OutCubic);
            }
        }
    }
}
