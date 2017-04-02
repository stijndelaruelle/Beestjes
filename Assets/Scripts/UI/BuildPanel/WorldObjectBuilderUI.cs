using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldObjectBuilderUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private World m_World;
    private string m_SelectedPrefab = "";

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_SelectedPrefab == "")
            return;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(eventData.pressPosition);
        worldPosition.z = 0.0f;

        m_World.SpawnWorldObject(m_SelectedPrefab, worldPosition);
        m_World.Serialize();

        m_SelectedPrefab = "";
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("DRAG");
    }

    public void SetSelectedPrefab(string prefabName)
    {
        m_SelectedPrefab = prefabName;
    }
}