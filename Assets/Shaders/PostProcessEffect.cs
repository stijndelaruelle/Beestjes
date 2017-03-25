using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PostProcessEffect : MonoBehaviour
{
    [SerializeField]
    private Material m_Material;

    private void OnRenderImage(RenderTexture sourceImage, RenderTexture destinationImage)
    {
        Graphics.Blit(sourceImage, destinationImage, m_Material);
    }
}
