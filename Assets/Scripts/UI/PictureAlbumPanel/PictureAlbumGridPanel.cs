using System;
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
    private PictureDisplayPanel m_SelectedPanel;

    [SerializeField]
    private RectTransform m_Content;

    public event PictureDelegate PictureSelectEvent;

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
        Refresh(DateTime.MinValue, DateTime.MaxValue);
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
            pictureDisplayPanel.PictureDisplaySelectedEvent -= OnPictureDisplaySelect;
            pictureDisplayPanel.PictureDisplayRemoveEvent -= OnPictureDisplayRemove;
            GameObject.Destroy(pictureDisplayPanel.gameObject);
        }

        m_PictureDisplayPanels.Clear();
    }

    public void AddFilter(DateTime fromDateTime, DateTime toDateTime)
    {
        Refresh(fromDateTime, toDateTime);
    }

    private void Refresh(DateTime fromDateTime, DateTime toDateTime)
    {
        List<Picture> pictures = m_PictureAlbum.Pictures;

        if (pictures == null)
            return;

        //Create / initialize displays
        int pictureCount = 0;
        for (int i = 0; i < pictures.Count; ++i)
        {
            Picture picture = pictures[i];

            //Timestamp check
            DateTime timeStamp = picture.TimeStamp;

            if (timeStamp >= fromDateTime && timeStamp <= toDateTime)
            {
                if (pictureCount >= m_PictureDisplayPanels.Count)
                {
                    AddPictureDisplay(picture);
                }
                else
                {
                    m_PictureDisplayPanels[pictureCount].Initialize(picture, m_QuestManager);
                }

                ++pictureCount;
            }
        }

        //Destroy any spare displays
        if (m_PictureDisplayPanels.Count > pictureCount)
        {
            for (int i = m_PictureDisplayPanels.Count - 1; i >= pictureCount; --i)
            {
                RemovePictureDisplay(i);
            }
        }

        //Select the first image in the list
        if (m_PictureDisplayPanels.Count > 0)
        {
            m_PictureDisplayPanels[0].Select();
        }
    }

    private void AddPictureDisplay(Picture picture)
    {
        PictureDisplayPanel newPanel = GameObject.Instantiate<PictureDisplayPanel>(m_PictureDisplayPanelPrefab, m_Content);
        newPanel.PictureDisplaySelectedEvent += OnPictureDisplaySelect;
        newPanel.PictureDisplayRemoveEvent += OnPictureDisplayRemove;
        newPanel.Initialize(picture, m_QuestManager);

        m_PictureDisplayPanels.Add(newPanel);
    }

    private void RemovePictureDisplay(int id)
    {
        if (id < 0 || id >= m_PictureDisplayPanels.Count)
            return;

        GameObject.Destroy(m_PictureDisplayPanels[id].gameObject);
        m_PictureDisplayPanels.RemoveAt(id);
    }

    public Picture GetSelectedPicture()
    {
        if (m_SelectedPanel == null)
            return null;

        return m_SelectedPanel.Picture;
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
                m_PictureDisplayPanels[i].PictureDisplaySelectedEvent -= OnPictureDisplaySelect;
                m_PictureDisplayPanels[i].PictureDisplayRemoveEvent -= OnPictureDisplayRemove;
                GameObject.Destroy(m_PictureDisplayPanels[i].gameObject);

                m_PictureDisplayPanels.RemoveAt(i);
            }
        }
    }

    //Picture Display Panel Events
    private void OnPictureDisplaySelect(PictureDisplayPanel pictureDisplayPanel)
    {
        if (m_SelectedPanel != null)
        {
            m_SelectedPanel.Deselect();
        }

        m_SelectedPanel = pictureDisplayPanel;

        //Pass trough
        if (PictureSelectEvent != null)
            PictureSelectEvent(pictureDisplayPanel.Picture);
    }

    private void OnPictureDisplayRemove(Picture picture)
    {
        m_PictureAlbum.RemovePicture(picture);
    }
}
