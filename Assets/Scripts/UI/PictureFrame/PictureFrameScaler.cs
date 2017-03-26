using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PictureFrameScaler : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private PictureFrame m_PictureFrame;

    [SerializeField]
    private bool m_ChangeWidth;

    [SerializeField]
    private bool m_ChangeHeight;

    [SerializeField]
    private bool m_ChangePositionWithWidth;

    [SerializeField]
    private bool m_ChangePositionWithHeight;

    public void OnDrag(PointerEventData eventData)
    {
        if (!m_ChangeWidth && !m_ChangeHeight)
            return;

        float xSign = 1.0f;
        if (m_ChangePositionWithWidth) xSign = -1.0f;

        float ySign = 1.0f;
        if (m_ChangePositionWithHeight) ySign = -1.0f;

        Vector2 addedSize = new Vector2();
        if (m_ChangeWidth) { addedSize.x = eventData.delta.x * xSign; }
        if (m_ChangeHeight) { addedSize.y = eventData.delta.y * ySign; }

        Vector2 usedValues = m_PictureFrame.AddSize(addedSize);

        //Change position (that comes with changing the width)
        if (m_ChangePositionWithWidth || m_ChangePositionWithHeight)
        {
            Vector2 addedPosition = new Vector2();

            if (m_ChangePositionWithWidth && m_ChangeWidth) { addedPosition.x = -usedValues.x; }
            if (m_ChangePositionWithHeight && m_ChangeHeight) { addedPosition.y = -usedValues.y; }

            m_PictureFrame.AddPosition(addedPosition);
        }
    }
}
