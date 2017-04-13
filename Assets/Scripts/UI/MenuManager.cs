using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    //TODO: Add history of panels
    private IVisible m_Active;

    public void Show(IVisible visibleObject)
    {
        if (m_Active != null)
        {
            m_Active.Hide(false);
        }

        visibleObject.Show();
        m_Active = visibleObject;
    }

    public void Hide(IVisible visibleObject)
    {
        visibleObject.Hide();
        m_Active = null;
    }
}
