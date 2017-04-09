using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastPictureDisplay : PictureDisplay
{
    [SerializeField]
    private PictureCamera m_PictureCamera;

    private void OnEnable()
    {
        Initialize(m_PictureCamera.LastPicture.Texture);
    }
}
