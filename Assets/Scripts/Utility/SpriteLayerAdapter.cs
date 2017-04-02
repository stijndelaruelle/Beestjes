using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof(SpriteRenderer))]
public class SpriteLayerAdapter : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_Offset;

    private SpriteRenderer m_SpriteRenderer;
	private void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        AdaptLayer();
    }

    #if (UNITY_EDITOR)
    private void Update()
    {
        AdaptLayer();
    }
    #endif

    protected void AdaptLayer()
    {
        int offset = 0;
        if (m_Offset != null)
            offset = m_Offset.sortingOrder;

        m_SpriteRenderer.sortingOrder = (int)Mathf.Abs(transform.position.y * 100.0f) + offset;
    }
}
