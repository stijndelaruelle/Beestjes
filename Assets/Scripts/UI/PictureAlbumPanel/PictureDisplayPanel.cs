using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureDisplayPanel : MonoBehaviour
{
    public delegate void PictureDisplayPanelDelegate(PictureDisplayPanel pictureDisplayPanel);
    [SerializeField]
    private Button m_Button;

    [SerializeField]
    private PictureDisplay m_PictureDisplay;
    private Picture m_Picture;
    public Picture Picture
    {
        get { return m_Picture; }
    }

    private QuestManager m_QuestManager;

    public event PictureDisplayPanelDelegate PictureDisplaySelectedEvent;
    public event PictureDelegate PictureDisplayRemoveEvent;

    public void Initialize(Picture picture, QuestManager QuestManager)
    {
        m_Picture = picture;
        m_QuestManager = QuestManager;
        m_PictureDisplay.Initialize(m_Picture.Texture);
    }

    public void Select()
    {
        m_Button.interactable = false;

        if (PictureDisplaySelectedEvent != null)
            PictureDisplaySelectedEvent(this);
    }

    public void Deselect()
    {
        m_Button.interactable = true;
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
