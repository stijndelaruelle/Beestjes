using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureAlbumGridPanel : MonoBehaviour
{
    [SerializeField]
    private PictureAlbum m_PictureAlbum;

    [SerializeField]
    private PictureDisplayPanel m_PictureDisplayPanelPrefab;
    private List<PictureDisplayPanel> m_PictureDisplayPanels;

    [SerializeField]
    private RectTransform m_Content;

    private void Awake()
    {
        m_PictureDisplayPanels = new List<PictureDisplayPanel>();
    }

    private void OnEnable()
    {
        List<Picture> pictures = m_PictureAlbum.Pictures;

        if (pictures == null)
            return;

        for (int i = 0; i < pictures.Count; ++i)
        {
            if (i >= m_PictureDisplayPanels.Count)
            {
                PictureDisplayPanel newPanel = GameObject.Instantiate<PictureDisplayPanel>(m_PictureDisplayPanelPrefab, m_Content);
                m_PictureDisplayPanels.Add(newPanel);
            }

            pictures[i].LoadTexture();
            m_PictureDisplayPanels[i].Initialize(pictures[i]);
        }
    }
}
