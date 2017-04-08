using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(RectTransform))]
public class InventoryPanel : MonoBehaviour
{
    [SerializeField]
    private Inventory m_Inventory;

    [SerializeField]
    private InventoryItemButton m_InventoryItemButtonPrefab;

    [SerializeField]
    private WorldObjectBuilderUI m_WorldObjectBuilder;

    [SerializeField]
    private PictureFrame m_PictureFrame;
    private RectTransform m_RectTransform;

    private List<InventoryItemButton> m_Buttons;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Buttons = new List<InventoryItemButton>();
    }

    private void Start()
    {
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        m_PictureFrame.VisibilityChangedEvent += OnPictureFrameVisibilityChanged;

        Initialize();
    }

    private void OnDestroy()
    {
        if (m_PictureFrame != null)
            m_PictureFrame.VisibilityChangedEvent -= OnPictureFrameVisibilityChanged;

        if (m_Inventory != null)
        {
            m_Inventory.ItemAddedEvent -= OnItemAdded;
            m_Inventory.ItemRemovedEvent -= OnItemRemoved;
        }
    }

    private void Initialize()
    {
        m_Inventory.ItemAddedEvent += OnItemAdded;
        m_Inventory.ItemRemovedEvent += OnItemRemoved;

        foreach (InventoryItem item in m_Inventory.Items)
        {
            AddButton(item);
        }
    }

    private void AddButton(InventoryItem item)
    {
        InventoryItemButton newButton = GameObject.Instantiate(m_InventoryItemButtonPrefab, m_RectTransform);
        newButton.Initialize(m_WorldObjectBuilder, item);

        m_Buttons.Add(newButton);
    }

    private void OnItemAdded(InventoryItem item)
    {
        AddButton(item);
    }

    private void OnItemRemoved(InventoryItem item)
    {
        for (int i = m_Buttons.Count - 1; i >= 0; --i)
        {
            if (m_Buttons[i].InventoryItem == item)
            {
                GameObject.Destroy(m_Buttons[i].gameObject);
                m_Buttons.Remove(m_Buttons[i]);
            }
        }
    }

    private void OnPictureFrameVisibilityChanged(bool value)
    {
        if (value)
        {
            m_RectTransform.DOAnchorPosY(m_RectTransform.rect.height, 0.5f).SetEase(Ease.OutCubic);
        }
        else
        {
            m_RectTransform.DOAnchorPosY(0.0f, 1).SetEase(Ease.OutBounce);
        }
    }
}
