using System;
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
    private PictureDisplay m_PictureDisplay;

    [SerializeField]
    private Text m_DeadlineText;

    [SerializeField]
    private CountdownTimerText m_CountdownTimerText;

    private void Start()
    {
        m_Quest.QuestSelectedPictureChangedEvent += OnQuestSelectedPictureChanged;
    }

    private void OnDestroy()
    {
        if (m_Quest != null)
            m_Quest.QuestSelectedPictureChangedEvent -= OnQuestSelectedPictureChanged;
    }

    public void Initialize(Quest Quest)
    {
        m_Quest = Quest;
        Refresh(m_Quest.SelectedPicture);
    }

    private void Refresh(Picture picture)
    {
        //Title
        m_Title.text = "Quest: " + m_Quest.QuestDefinition.Title;

        //Picture
        if (picture == null)
            m_PictureDisplay.Initialize(null);
        else
            m_PictureDisplay.Initialize(picture.Texture);

        //Deadline
        DateTime deadline = m_Quest.Deadline;
        m_DeadlineText.text = deadline.ToString("dd/MM/yyyy HH:mm:ss");
        m_CountdownTimerText.SetTargetTime(deadline);
    }

    //Events
    private void OnQuestSelectedPictureChanged(Picture picture)
    {
        Refresh(picture);
    }

}
