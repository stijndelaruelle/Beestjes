using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagazinePicturePanel : MonoBehaviour
{
    [SerializeField]
    private MagazineManager m_MagazineManager;

    [SerializeField]
    private PictureDisplay m_PictureDisplay;

    [SerializeField]
    private Text m_InfoText;

    private void Start()
    {
        m_MagazineManager.MagazinePictureChangedEvent += OnMagazinePictureChanged;
    }

    private void OnDestroy()
    {
        if (m_MagazineManager != null)
            m_MagazineManager.MagazinePictureChangedEvent -= OnMagazinePictureChanged;
    }

    private void OnEnable()
    {
        Refresh(m_MagazineManager.Picture);
    }

    private void Refresh(Picture picture)
    {
        if (picture == null)
            m_PictureDisplay.Initialize(null);
        else
            m_PictureDisplay.Initialize(picture.Texture);
    }

    //Events
    private void OnMagazinePictureChanged(Picture picture)
    {
        Refresh(picture);
    }
}
