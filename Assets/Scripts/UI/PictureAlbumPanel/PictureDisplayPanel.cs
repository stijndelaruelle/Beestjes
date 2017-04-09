using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureDisplayPanel : MonoBehaviour
{
    [SerializeField]
    private PictureDisplay m_PictureDisplay;
    private Picture m_Picture;

    public void Initialize(Picture picture)
    {
        m_Picture = picture;
        m_PictureDisplay.Initialize(m_Picture.Texture);
    }

    public void Share()
    {
        SharePlugin.ShareImage("Share picture...", m_Picture.TextureFilePath, "png");
    }
}
