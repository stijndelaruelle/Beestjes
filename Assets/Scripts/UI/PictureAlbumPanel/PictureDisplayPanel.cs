using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureDisplayPanel : MonoBehaviour
{
    [SerializeField]
    private PictureDisplay m_PictureDisplay;
    private Picture m_Picture;
    public Picture Picture
    {
        get { return m_Picture; }
    }

    private QuestManager m_QuestManager;

    public event PictureDelegate PictureDisplayRemoveEvent;

    public void Initialize(Picture picture, QuestManager QuestManager)
    {
        m_Picture = picture;
        m_QuestManager = QuestManager;
        m_PictureDisplay.Initialize(m_Picture.Texture);
    }

    public void Share()
    {
        SharePlugin.ShareImage("Share picture...", m_Picture.TextureFilePath, "png");
    }

    public void SelectForQuest()
    {
        m_QuestManager.SetPicture(m_Picture);
    }

    public void Delete()
    {
        if (PictureDisplayRemoveEvent != null)
            PictureDisplayRemoveEvent(m_Picture);
    }
}
