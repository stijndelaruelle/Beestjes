using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPictureSelectForQuestButton : MonoBehaviour
{
    [SerializeField]
    private PictureCamera m_PictureCamera;

    [SerializeField]
    private LastPictureTakenPanel m_PictureTakenPanel;

    [SerializeField]
    private QuestManager m_QuestManager;

    private void OnDestroy()
    {
        if (m_PictureCamera != null)
            m_PictureCamera.PictureSavedEvent -= OnPictureSaved;
    }

    public void SelectForQuest()
    {
        //Save
        m_PictureCamera.PictureSavedEvent += OnPictureSaved;
        m_PictureCamera.SavePictureToDisk();
    }

    private void OnPictureSaved(Picture picture)
    {
        m_QuestManager.SetPicture(picture);

        //Hide window
        m_PictureTakenPanel.Hide();

        if (m_PictureCamera != null)
            m_PictureCamera.PictureSavedEvent -= OnPictureSaved;
    }
}
