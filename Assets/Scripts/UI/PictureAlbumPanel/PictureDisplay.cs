using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(RawImage))]
public class PictureDisplay : MonoBehaviour
{
    protected RectTransform m_RectTransform;
    protected RawImage m_RawImage;
    protected float m_InitialSize;

    public void Initialize(Texture2D texture)
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_RawImage = GetComponent<RawImage>();
        m_InitialSize = m_RectTransform.sizeDelta.x;

        if (texture == null)
        {
            m_RawImage.texture = null;
            return;
        }

        //Set the picture
        m_RawImage.texture = texture;

        //Set the frame to the correct scale
        float aspectRatio = (float)m_RawImage.texture.width / (float)m_RawImage.texture.height;

        float newWidth = m_InitialSize;
        float newHeight = m_InitialSize;

        if (aspectRatio > 1.0f) { newHeight = (1.0f / aspectRatio) * m_InitialSize; }
        if (aspectRatio < 1.0f) { newWidth = aspectRatio * m_InitialSize; }

        m_RectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }
}
