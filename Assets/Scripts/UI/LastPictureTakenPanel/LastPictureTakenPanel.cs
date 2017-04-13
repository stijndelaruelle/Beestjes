using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPictureTakenPanel : IVisible
{
    [SerializeField]
    private PictureCamera m_PictureCamera;

    public void Start()
    {
        m_PictureCamera.PictureTakenEvent += OnPictureTaken;
        Hide();
    }

    private void OnDestroy()
    {
        if (m_PictureCamera != null)
            m_PictureCamera.PictureTakenEvent -= OnPictureTaken;
    }

    private void OnPictureTaken(Picture picture)
    {
        Show();
    }
}
