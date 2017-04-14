using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : IVisible
{
    private Stack<IVisible> m_PanelStack;

    private void Awake()
    {
        m_PanelStack = new Stack<IVisible>();
    }

    public void Show(IVisible visibleObject)
    {
        if (m_PanelStack.Count > 0)
        {
            Hide(m_PanelStack.Peek());
        }

        visibleObject.Show();

        m_PanelStack.Push(visibleObject);

        if (m_PanelStack.Count == 1)
            FireVisibilityChangedEvent(true);
    }

    public void Hide(IVisible visibleObject)
    {
        visibleObject.Hide();

        if (m_PanelStack.Count > 0)
        {
            m_PanelStack.Pop();

            if (m_PanelStack.Count == 0)
                FireVisibilityChangedEvent(false);
        }
    }
}
