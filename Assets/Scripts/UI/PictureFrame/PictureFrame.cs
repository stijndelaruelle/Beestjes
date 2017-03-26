using UnityEngine;

[RequireComponent (typeof(RectTransform))]
public class PictureFrame : MonoBehaviour
{
    private RectTransform m_RectTransform;

    [SerializeField]
    private Camera m_Camera;

    [SerializeField]
    private Canvas m_Canvas;
    public Canvas Canvas
    {
        get { return m_Canvas; }
    }

    [MinMaxRange(1, 500)]
    [SerializeField]
    private MinMaxRange m_SizeRange;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    //Mutators
    public void AddPosition(Vector2 position)
    {
        Vector3 currentPosition = m_RectTransform.position;
        currentPosition += new Vector3(position.x, position.y);

        m_RectTransform.position = currentPosition;
    }

    public Vector2 AddSize(Vector2 size)
    {
        float sf = m_Canvas.scaleFactor;

        Vector2 usedValues = new Vector2();

        Vector2 currentSize = m_RectTransform.sizeDelta * sf;
        currentSize.x += size.x;
        currentSize.y += size.y;

        //Clamp values
        float scaledMin = m_SizeRange.Min * sf;
        float scaledMax = m_SizeRange.Max * sf;

        currentSize.x = Mathf.Clamp(currentSize.x, scaledMin, scaledMax);
        usedValues.x = (currentSize.x - (m_RectTransform.sizeDelta.x * sf));

        currentSize.y = Mathf.Clamp(currentSize.y, scaledMin, scaledMax);
        usedValues.y = (currentSize.y - (m_RectTransform.sizeDelta.y * sf));

        m_RectTransform.sizeDelta = currentSize / sf;

        return usedValues;
    }
    
    //Accessors
    public Rect GetRect()
    {
        Vector2 pos = GetPosition();
        Vector2 size = GetSize();

        return new Rect(pos.x, pos.y, size.x, size.y);
    }

    public Vector2 GetPosition()
    {
        return m_RectTransform.position;
    }

    public Vector2 GetSize()
    {
        return new Vector2(m_RectTransform.sizeDelta.x * m_Canvas.scaleFactor,
                           m_RectTransform.sizeDelta.y * m_Canvas.scaleFactor);
    }
}
