using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompetitionPicturePanel : MonoBehaviour
{
    [SerializeField]
    private Competition m_Competition;

    [SerializeField]
    private PictureDisplay m_PictureDisplay;

    [SerializeField]
    private Text m_InfoText;

    private void Start()
    {
        m_Competition.CompetitionPictureChangedEvent += OnCompetitionPictureChanged;
    }

    private void OnDestroy()
    {
        if (m_Competition != null)
            m_Competition.CompetitionPictureChangedEvent -= OnCompetitionPictureChanged;
    }

    private void OnEnable()
    {
        Refresh(m_Competition.Picture);
    }

    private void Refresh(Picture picture)
    {
        if (picture == null)
            m_PictureDisplay.Initialize(null);
        else
            m_PictureDisplay.Initialize(picture.Texture);
    }

    //Events
    private void OnCompetitionPictureChanged(Picture picture)
    {
        Refresh(picture);
    }
}
