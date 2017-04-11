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

    private Competition m_Competition;

    public event PictureDelegate PictureDisplayRemoveEvent;

    public void Initialize(Picture picture, Competition competition)
    {
        m_Picture = picture;
        m_Competition = competition;
        m_PictureDisplay.Initialize(m_Picture.Texture);
    }

    public void Share()
    {
        SharePlugin.ShareImage("Share picture...", m_Picture.TextureFilePath, "png");
    }

    public void EnterCompetition()
    {
        m_Competition.SetPicture(m_Picture);
    }

    public void Delete()
    {
        if (PictureDisplayRemoveEvent != null)
            PictureDisplayRemoveEvent(m_Picture);
    }
}
