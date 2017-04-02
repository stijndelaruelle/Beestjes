using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ShowPictureFrameButton : MonoBehaviour
{
    [SerializeField]
    private PictureFrame m_PictureFrame;
    private RectTransform m_RectTransform;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        m_PictureFrame.VisibilityChangedEvent += OnPictureFrameVisibilityChanged;
    }

    private void OnDestroy()
    {
        if (m_PictureFrame != null)
            m_PictureFrame.VisibilityChangedEvent -= OnPictureFrameVisibilityChanged;
    }

    public void ShowPictureFrame()
    {
        m_PictureFrame.gameObject.SetActive(true);
    }

    private void OnPictureFrameVisibilityChanged(bool value)
    {
        if (value)
        {
            m_RectTransform.DOAnchorPosY(-m_RectTransform.rect.height, 0.5f).SetEase(Ease.OutCubic);
        }
        else
        {
            m_RectTransform.DOAnchorPosY(0.0f, 1).SetEase(Ease.OutBounce);
        }
    }
}
