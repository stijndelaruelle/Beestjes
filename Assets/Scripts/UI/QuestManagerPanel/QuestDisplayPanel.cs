using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplayPanel : MonoBehaviour
{
    private Quest m_Quest;

    [SerializeField]
    private Text m_Title;

    [SerializeField]
    private Text m_InfoText;

    [SerializeField]
    private PictureDisplay m_PictureDisplay;

    public void Initialize(Quest Quest)
    {
        m_Quest = Quest;
        Refresh(m_Quest.SelectedPicture);
    }

    private void Start()
    {
        m_Quest.QuestSelectedPictureChangedEvent += OnQuestSelectedPictureChanged;
    }

    private void OnDestroy()
    {
        if (m_Quest != null)
            m_Quest.QuestSelectedPictureChangedEvent -= OnQuestSelectedPictureChanged;
    }

    private void Refresh(Picture picture)
    {
        if (picture == null)
            m_PictureDisplay.Initialize(null);
        else
            m_PictureDisplay.Initialize(picture.Texture);
    }

    //Events
    private void OnQuestSelectedPictureChanged(Picture picture)
    {
        Refresh(picture);
    }

}
