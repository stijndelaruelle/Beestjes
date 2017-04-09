using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPictureShareButton : MonoBehaviour
{
    [SerializeField]
    private PictureCamera m_PictureCamera;

    [SerializeField]
    private LastPictureTakenPanel m_PictureTakenPanel;

    private void OnDestroy()
    {
        if (m_PictureCamera != null)
            m_PictureCamera.PictureSavedEvent -= OnPictureSaved;
    }

    public void Share()
    {
        //Save
        m_PictureCamera.PictureSavedEvent += OnPictureSaved;
        m_PictureCamera.SavePictureToDisk();
    }

    private void OnPictureSaved(Picture picture)
    {
        //Share
        SharePlugin.ShareImage("Share picture...", picture.TextureFilePath, "png");

        //Hide window
        m_PictureTakenPanel.Hide();

        if (m_PictureCamera != null)
            m_PictureCamera.PictureSavedEvent -= OnPictureSaved;
    }
}
