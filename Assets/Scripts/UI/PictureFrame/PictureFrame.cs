using UnityEngine;

[RequireComponent (typeof(RectTransform))]
public class PictureFrame : MonoBehaviour
{
    public delegate void BoolDelegate(bool value);

    private RectTransform m_RectTransform;

    [SerializeField]
    private Camera m_PictureCamera;

    [SerializeField]
    private Canvas m_Canvas;
    public Canvas Canvas
    {
        get { return m_Canvas; }
    }

    [MinMaxRange(1, 1000)]
    [SerializeField]
    private MinMaxRange m_SizeRangeWidth;

    [MinMaxRange(1, 1000)]
    [SerializeField]
    private MinMaxRange m_SizeRangeHeight;

    public event BoolDelegate VisibilityChangedEvent;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (VisibilityChangedEvent != null)
            VisibilityChangedEvent(true);
    }

    private void OnDisable()
    {
        if (VisibilityChangedEvent != null)
            VisibilityChangedEvent(false);
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
        currentSize.x = Mathf.Clamp(currentSize.x, m_SizeRangeWidth.Min * sf, m_SizeRangeWidth.Max * sf);
        usedValues.x = (currentSize.x - (m_RectTransform.sizeDelta.x * sf));

        currentSize.y = Mathf.Clamp(currentSize.y, m_SizeRangeHeight.Min * sf, m_SizeRangeHeight.Max * sf);
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
