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

    private Texture2D m_LastPicture;
    public Texture2D LastPicture
    {
        get { return m_LastPicture; }
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
        //If you forget this line, the android build will render the UI below all the camera's...
        Graphics.Blit(src, dest);

        if (m_TakePicture == false)
            return;

        int width = src.width;
        int height = src.height;

        //Create and activate a new rendertexture
        //RenderTexture renderTexture = new RenderTexture(width, height, 24);
        //m_Camera.targetTexture = renderTexture;
        //m_Camera.Render();

        RenderTexture.active = src;

        //Create a new texture & read all pixels from the rendertexture
        Texture2D picture = new Texture2D(width, height, TextureFormat.RGB24, false);
        picture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        //#if UNITY_EDITOR
        //    //Flip the picture on the X axis (Not needed on android)
        //    Color[] origColours = picture.GetPixels();
        //    Color[] flippedColours = new Color[origColours.Length];

        //    for (int x = 0; x < width; ++x)
        //    {
        //        for (int y = 0; y < height; ++y)
        //        {
        //            flippedColours[x + (y * width)] = origColours[x + ((height - y - 1) * width)];
        //        }
        //    }

        //    picture.SetPixels(flippedColours);
        //#endif

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

        string path = SaveGameManager.Instance.SavePictureToDisk(m_LastPicture);

        m_PictureSaved = true;

        if (PictureSavedEvent != null)
            PictureSavedEvent(path);

        Debug.Log("Picture saved!");
    }
}
