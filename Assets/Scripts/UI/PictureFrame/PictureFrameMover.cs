using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PictureFrameMover : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private PictureFrame m_PictureFrame;

    public void OnDrag(PointerEventData eventData)
    {
        m_PictureFrame.AddPosition(eventData.delta);
    }
}
