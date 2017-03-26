using System.Collections;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PictureCamera : MonoBehaviour
{
    public delegate void PictureDelegate(Texture2D picture);
    public delegate void PathDelegate(string path);

    private Camera m_Camera;

    [SerializeField]
    private PictureFrame m_PictureFrame;

    [SerializeField]
    private string m_PictureFolder;

    private Texture2D m_LastPicture;
    public Texture2D LastPicture
    {
        get { return m_LastPicture; }
    }

    private string m_LastPicturePath;
    public string LastPicturePath
    {
        get { return m_LastPicturePath; }
    }

    private bool m_TakePicture;
    private bool m_PictureSaved;

    public event PictureDelegate PictureTakenEvent;
    public event PathDelegate PictureSavedEvent;

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

        int width = src.width;
        int height = src.height;

        RenderTexture.active = src;

        Texture2D picture = new Texture2D(width, height, TextureFormat.RGB24, false);

        //Flip the picture on the X axis
        picture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Color[] origColours = picture.GetPixels();
        Color[] flippedColours = new Color[origColours.Length];

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                flippedColours[x + (y * width)] = origColours[x + ((height - y - 1) * width)];
            }
        }

        picture.SetPixels(flippedColours);
        picture.Apply();

        RenderTexture.active = null;

        m_LastPicture = picture;
        m_TakePicture = false;
        m_PictureSaved = false;

        if (PictureTakenEvent != null)
            PictureTakenEvent(picture);

        Debug.Log("Picture taken!");
    }

    public void SavePictureToDisk()
    {
        if (m_LastPicture == null)
            return;

        //Avoids saving the same picture multiple times
        if (m_PictureSaved)
            return;

        byte[] bytes;
        bytes = m_LastPicture.EncodeToPNG();

        string folderPath = m_PictureFolder;

        #if UNITY_ANDROID && !UNITY_EDITOR
            folderPath = DataPathPlugin.GetFirstWriteableExternalDataPath();
            Debug.Log("Chosen folder: " + folderPath);
        #endif

        string path = string.Format("{0}/picture_{1}.png", folderPath, System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss"));

        File.WriteAllBytes(path, bytes);

        m_LastPicturePath = path;
        m_PictureSaved = true;

        if (PictureSavedEvent != null)
            PictureSavedEvent(path);

        Debug.Log("Picture saved!");
    }
}
