using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMoonRotateUpwards : MonoBehaviour
{
    [SerializeField]
    private SunMoonRotator m_Rotator;

    private void Awake()
    {
        m_Rotator.RefreshEvent += OnRotatorRefresh;
    }

    private void OnDestroy()
    {
        if (m_Rotator != null)
            m_Rotator.RefreshEvent += OnRotatorRefresh;
    }

    private void OnRotatorRefresh()
    {
        Vector3 rotatorEuler = m_Rotator.transform.localRotation.eulerAngles;
        rotatorEuler.z *= -1.0f;

        transform.localRotation *= Quaternion.Euler(rotatorEuler);
    }
}
