using System.Collections;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PictureCamera : MonoBehaviour
{
    public delegate void PictureDelegate(Texture2D picture);

    private Camera m_Camera;

    [SerializeField]
    private PictureFrame m_PictureFrame;

    [SerializeField]
    private string m_PictureFolder;
    private Texture2D m_LastPicture;

    private bool m_TakePicture;

    public event PictureDelegate PictureTakenEvent;


    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    private void Update()
    {
        Rect frameRect = m_PictureFrame.GetRect();

        //Set the camera to the right position in the viewport
        m_Camera.pixelRect = frameRect;

        //Set the camera to the right position in the world
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(frameRect.center);
        worldPoint.z = -10.0f;
        m_Camera.transform.position = worldPoint;

        //Scale the camera correctly
        float orthographicSize = (m_Camera.rect.height) * Camera.main.orthographicSize;
        m_Camera.orthographicSize = orthographicSize;
    }

    public void TakePicture()
    {
        m_TakePicture = true;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!m_TakePicture)
            return;

        int width = src.width; // m_Camera.pixelWidth;
        int height = src.height; // m_Camera.pixelHeight;

        //RenderTexture renderTexture = new RenderTexture(width, height, 24);
        //m_Camera.targetTexture = renderTexture;
        //m_Camera.Render();

        RenderTexture.active = src;

        Texture2D picture = new Texture2D(width, height, TextureFormat.RGB24, false);
        picture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        picture.Apply();

        RenderTexture.active = null;
        //m_Camera.targetTexture = null;

        m_LastPicture = picture;
        Debug.Log("Picture taken!");

        if (PictureTakenEvent != null)
            PictureTakenEvent(picture);

        //////Destroy(tempRT); - tricky on android and other platforms, take care
        m_TakePicture = false;
    }

    public void SavePictureToDisk()
    {
        if (m_LastPicture == null)
            return;

        byte[] bytes;
        bytes = m_LastPicture.EncodeToPNG();

        string path = string.Format("{0}/picture_{1}.png", m_PictureFolder, System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss"));

        File.WriteAllBytes(path, bytes);

        Debug.Log("Picture saved!");
    }
}
