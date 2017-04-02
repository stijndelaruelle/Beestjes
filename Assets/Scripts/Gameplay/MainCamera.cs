using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private PictureFrame m_PictureFrame;

    [SerializeField]
    private PostProcessingBehaviour m_PostProcessEffect;

    private void Start()
    {
        m_PostProcessEffect.enabled = false;
        m_PictureFrame.VisibilityChangedEvent += OnPictureFrameVisibilityChanged;
    }

    private void OnDestroy()
    {
        if (m_PictureFrame != null)
            m_PictureFrame.VisibilityChangedEvent -= OnPictureFrameVisibilityChanged;
    }

    private void OnPictureFrameVisibilityChanged(bool value)
    {
        m_PostProcessEffect.enabled = value;
    }
}
