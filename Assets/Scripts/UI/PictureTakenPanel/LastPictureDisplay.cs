using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(RectTransform))]
[RequireComponent (typeof(RawImage))]
public class LastPictureDisplay : MonoBehaviour
{
    private RectTransform m_RectTransform;
    private RawImage m_RawImage;

    [SerializeField]
    private PictureCamera m_PictureCamera;
    private float m_InitialSize;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_RawImage = GetComponent<RawImage>();
        m_InitialSize = m_RectTransform.sizeDelta.x;
    }

    private void OnEnable()
    {
        if (m_PictureCamera.LastPicture == null)
            return;

        //Set the picture
        m_RawImage.texture = m_PictureCamera.LastPicture.Texture;

        //Set the frame to the correct scale
        float aspectRatio = (float)m_RawImage.texture.width / (float)m_RawImage.texture.height;

        float newWidth = m_InitialSize;
        float newHeight = m_InitialSize;

        if (aspectRatio > 1.0f) { newHeight = (1.0f / aspectRatio) * m_InitialSize; }
        if (aspectRatio < 1.0f) { newWidth = aspectRatio * m_InitialSize; }

        m_RectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }
}
