using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplayPanel : MonoBehaviour
{
    public delegate void QuestDisplayPanelDelegate(QuestDisplayPanel questDisplayPanel);

    private Quest m_Quest;
    public Quest Quest
    {
        get { return m_Quest; }
    }

    [SerializeField]
    private Text m_Title;

    [SerializeField]
    private PictureDisplay m_PictureDisplay;

    [SerializeField]
    private Text m_Description;

    [SerializeField]
    private Text m_DeadlineText;

    [SerializeField]
    private CountdownTimerText m_CountdownTimerText;

    [SerializeField]
    private Button m_Button;

    public event QuestDisplayPanelDelegate QuestDisplayPanelSelectEvent;

    private void OnDestroy()
    {
        if (m_Quest != null)
            m_Quest.QuestSelectedPictureChangedEvent -= OnQuestSelectedPictureChanged;
    }

    public void Initialize(Quest Quest)
    {
        if (m_Quest != null)
        {
            m_Quest.QuestSelectedPictureChangedEvent -= OnQuestSelectedPictureChanged;
        }

        m_Quest = Quest;
        m_Quest.QuestSelectedPictureChangedEvent += OnQuestSelectedPictureChanged;

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

        //Description
        m_Description.text = m_Quest.QuestDefinition.Description;

        //Deadline
        DateTime deadline = m_Quest.EndTime;
        m_DeadlineText.text = deadline.ToString("dd/MM/yyyy HH:mm:ss");
        m_CountdownTimerText.SetTargetTime(deadline);
    }

    public void Select()
    {
        m_Button.interactable = false;

        if (QuestDisplayPanelSelectEvent != null)
            QuestDisplayPanelSelectEvent(this);
    }

    public void Deselect()
    {
        m_Button.interactable = true;
    }

    //Events
    private void OnQuestSelectedPictureChanged(Picture picture)
    {
        Refresh(picture);
    }

}
