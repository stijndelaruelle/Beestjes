using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureAlbumPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Visuals; //Workaround so this object can subscribe itself to the picture camera event.

    public void Start()
    {
        Hide();
    }

    public void Show()
    {
        m_Visuals.SetActive(true);
    }

    public void Hide()
    {
        m_Visuals.SetActive(false);
    }
}
