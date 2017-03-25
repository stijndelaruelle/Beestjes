using System.Collections;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PictureCamera : MonoBehaviour
{
    private Camera m_Camera;

    [SerializeField]
    private PictureFrame m_PictureFrame;

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
        StartCoroutine(TakeScreenShotRoutine());
    }

    public IEnumerator TakeScreenShotRoutine()
    {
        yield return new WaitForEndOfFrame(); // it must be a coroutine 

        int width = m_Camera.pixelWidth;
        int height = m_Camera.pixelHeight;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        var bytes = tex.EncodeToPNG();
        Destroy(tex);

        File.WriteAllBytes("./ScreenShot.png", bytes);

        Debug.Log("Picture taken!");
    }
}
