using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagazineDisplayPanel : MonoBehaviour
{
    private Magazine m_Magazine;

    [SerializeField]
    private Text m_Title;

    [SerializeField]
    private Text m_InfoText;

    [SerializeField]
    private PictureDisplay m_PictureDisplay;

    public void Initialize(Magazine magazine)
    {
        m_Magazine = magazine;
        Refresh(m_Magazine.Picture);
    }

    private void Start()
    {
        m_Magazine.MagazinePictureChangedEvent += OnMagazinePictureChanged;
    }

    private void OnDestroy()
    {
        if (m_Magazine != null)
            m_Magazine.MagazinePictureChangedEvent -= OnMagazinePictureChanged;
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
