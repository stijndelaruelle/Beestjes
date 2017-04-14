using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivePicturePanel : MonoBehaviour
{
    [SerializeField]
    private PictureAlbumGridPanel m_PictureAlbumGridPanel;

    [SerializeField]
    private PictureDisplay m_PictureDisplay;

    [SerializeField]
    private Text m_DateText;

    private void Start()
    {
        m_PictureAlbumGridPanel.PictureSelectEvent += OnPictureSelect;
        Refresh(m_PictureAlbumGridPanel.GetSelectedPicture());
    }

    private void OnDestroy()
    {
        if (m_PictureAlbumGridPanel != null)
            m_PictureAlbumGridPanel.PictureSelectEvent -= OnPictureSelect;
    }

    private void Refresh(Picture picture)
    {
        m_PictureDisplay.Initialize(picture.Texture);
        m_DateText.text = "Taken on " + picture.TimeStamp.ToString("dd/MM/yyyy HH:mm:ss");
    }

    //Events
    private void OnPictureSelect(Picture picture)
    {
        Refresh(picture);
    }
}
