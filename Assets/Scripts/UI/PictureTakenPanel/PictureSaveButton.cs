using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureSaveButton : MonoBehaviour
{
    [SerializeField]
    private PictureCamera m_PictureCamera;

    [SerializeField]
    private PictureTakenPanel m_PictureTakenPanel;

    private void Awake()
    {
        m_PictureCamera.PictureSavedEvent += OnPictureSaved;
    }

    private void OnDestroy()
    {
        if (m_PictureCamera != null)
            m_PictureCamera.PictureSavedEvent -= OnPictureSaved;
    }

    public void Save()
    {
        //Save
        m_PictureCamera.SavePictureToDisk();
    }

    private void OnPictureSaved(string path)
    {
        //Hide window
        m_PictureTakenPanel.Hide();
    }
}
