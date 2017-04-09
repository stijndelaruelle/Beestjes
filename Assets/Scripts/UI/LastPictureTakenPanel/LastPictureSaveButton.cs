using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPictureSaveButton : MonoBehaviour
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

    public void Save()
    {
        //Save
        m_PictureCamera.PictureSavedEvent += OnPictureSaved;
        m_PictureCamera.SavePictureToDisk();
    }

    private void OnPictureSaved(Picture picture)
    {
        //Hide window
        m_PictureTakenPanel.Hide();

        if (m_PictureCamera != null)
            m_PictureCamera.PictureSavedEvent -= OnPictureSaved;
    }
}
