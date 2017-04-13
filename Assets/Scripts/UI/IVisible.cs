using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVisible : MonoBehaviour
{
    public delegate void BoolDelegate(bool value);

    [SerializeField]
    protected RectTransform m_Visuals;

    public event BoolDelegate VisibilityChangedEvent;

    public void Show()
    {
        SetVisibility(true, true);
    }

    public void Hide()
    {
        SetVisibility(false, true);
    }

    public void Show(bool fireEvent)
    {
        SetVisibility(true, fireEvent);
    }

    public void Hide(bool fireEvent)
    {
        SetVisibility(false, fireEvent);
    }

    public virtual void SetVisibility(bool value, bool fireEvent)
    {
        m_Visuals.gameObject.SetActive(value);

        if (fireEvent)
            FireVisibilityChangedEvent(value);
    }

    protected void FireVisibilityChangedEvent(bool value)
    {
        if (VisibilityChangedEvent != null)
            VisibilityChangedEvent(value);
    }
}
