using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PictureFrameTakePictureButton : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private PictureCamera m_PictureCamera;
    private bool m_TakePicture = true;

    public void OnDrag(PointerEventData eventData)
    {
        m_TakePicture = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_TakePicture = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //We dragged the object, so we're not taking a picture
        if (m_TakePicture == false)
            return;

        m_PictureCamera.TakePicture();
    }
}
