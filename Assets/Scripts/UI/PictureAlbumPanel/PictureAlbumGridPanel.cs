using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureAlbumGridPanel : MonoBehaviour
{
    [SerializeField]
    private PictureAlbum m_PictureAlbum;

    [SerializeField]
    private QuestManager m_QuestManager;

    [SerializeField]
    private PictureDisplayPanel m_PictureDisplayPanelPrefab;
    private List<PictureDisplayPanel> m_PictureDisplayPanels;

    [SerializeField]
    private RectTransform m_Content;

    private void Awake()
    {
        m_PictureDisplayPanels = new List<PictureDisplayPanel>();
    }

    private void Start()
    {
        if (m_PictureAlbum == null)
            return;

        m_PictureAlbum.PictureAddEvent += OnPictureAdd;
        m_PictureAlbum.PictureEditEvent += OnPictureEdit;
        m_PictureAlbum.PictureRemoveEvent += OnPictureRemove;
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
                AddPictureDisplay(pictures[i]);
            }
            else
            {
                m_PictureDisplayPanels[i].Initialize(pictures[i], m_QuestManager);
            }
        }
    }

    private void OnDestroy()
    {
        //Unsubscribe
        if (m_PictureAlbum != null)
        {
            m_PictureAlbum.PictureAddEvent -= OnPictureAdd;
            m_PictureAlbum.PictureEditEvent -= OnPictureEdit;
            m_PictureAlbum.PictureRemoveEvent -= OnPictureRemove;
        }

        foreach (PictureDisplayPanel pictureDisplayPanel in m_PictureDisplayPanels)
        {
            pictureDisplayPanel.PictureDisplayRemoveEvent -= OnPictureDisplayRemove;
            GameObject.Destroy(pictureDisplayPanel.gameObject);
        }

        m_PictureDisplayPanels.Clear();
    }

    private void AddPictureDisplay(Picture picture)
    {
        PictureDisplayPanel newPanel = GameObject.Instantiate<PictureDisplayPanel>(m_PictureDisplayPanelPrefab, m_Content);
        newPanel.PictureDisplayRemoveEvent += OnPictureDisplayRemove;
        newPanel.Initialize(picture, m_QuestManager);

        m_PictureDisplayPanels.Add(newPanel);
    }

    //Picture Album Events
    private void OnPictureAdd(Picture picture)
    {
        AddPictureDisplay(picture);
    }

    private void OnPictureEdit(Picture picture)
    {

    }

    private void OnPictureRemove(Picture picture)
    {
        for (int i = m_PictureDisplayPanels.Count - 1; i >= 0; --i)
        {
            if (m_PictureDisplayPanels[i].Picture == picture)
            {
                m_PictureDisplayPanels[i].PictureDisplayRemoveEvent -= OnPictureDisplayRemove;
                GameObject.Destroy(m_PictureDisplayPanels[i].gameObject);

                m_PictureDisplayPanels.RemoveAt(i);
            }
        }
    }

    //Picture Display Panel Events
    private void OnPictureDisplayRemove(Picture picture)
    {
        m_PictureAlbum.RemovePicture(picture);
    }
}
