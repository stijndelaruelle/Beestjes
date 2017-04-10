using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPanel : MonoBehaviour
{
    //[SerializeField]
    //private PictureCamera m_PictureCamera;

    [SerializeField]
    private GameObject m_Visuals; //Workaround so this object can subscribe itself to the picture camera event.

    public void Start()
    {
        //m_PictureCamera.PictureTakenEvent += OnPictureTaken;
        Hide();
    }

    private void OnDestroy()
    {
        //if (m_PictureCamera != null)
        //    m_PictureCamera.PictureTakenEvent -= OnPictureTaken;
    }

    //private void OnPictureTaken(Picture picture)
    //{
    //    Show();
    //}

    public void Show()
    {
        m_Visuals.SetActive(true);
    }

    public void Hide()
    {
        m_Visuals.SetActive(false);
    }
}
