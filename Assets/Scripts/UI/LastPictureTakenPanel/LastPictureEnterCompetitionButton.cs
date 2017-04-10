using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPictureEnterCompetitionButton : MonoBehaviour
{
    [SerializeField]
    private PictureCamera m_PictureCamera;

    [SerializeField]
    private LastPictureTakenPanel m_PictureTakenPanel;

    [SerializeField]
    private Competition m_Competition;

    private void OnDestroy()
    {
        if (m_PictureCamera != null)
            m_PictureCamera.PictureSavedEvent -= OnPictureSaved;
    }

    public void EnterCompetition()
    {
        //Save
        m_PictureCamera.PictureSavedEvent += OnPictureSaved;
        m_PictureCamera.SavePictureToDisk();
    }

    private void OnPictureSaved(Picture picture)
    {
        //Enter competition
        m_Competition.SetPicture(picture);

        //Hide window
        m_PictureTakenPanel.Hide();

        if (m_PictureCamera != null)
            m_PictureCamera.PictureSavedEvent -= OnPictureSaved;
    }
}
