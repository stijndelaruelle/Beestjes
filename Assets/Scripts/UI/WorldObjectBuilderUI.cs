using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldObjectBuilderUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private World m_World;
    private InventoryItem m_SelectedItem;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_SelectedItem == null)
            return;

        if (m_SelectedItem.CanUse() == false)
            return;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(eventData.pressPosition);
        worldPosition.z = 0.0f;

        WorldObject newWorldObject = m_World.SpawnWorldObject(m_SelectedItem.WorldObjectPrefab, worldPosition);
        newWorldObject.Initialize(GameClock.Instance.GetDateTime());

        m_SelectedItem.Use();
        m_SelectedItem = null;

        SaveGameManager.Instance.SerializeWorld();
        SaveGameManager.Instance.SerializeInventory();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("DRAG");
    }

    public void SelectItem(InventoryItem inventoryItem)
    {
        m_SelectedItem = inventoryItem;
    }
}