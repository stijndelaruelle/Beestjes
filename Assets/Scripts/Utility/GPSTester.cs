using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSTester : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    private void Start()
    {
        m_Text.text = "LON: " + GPSPlugin.GetLongitude() + ", LAT: " + GPSPlugin.GetLatitude();
    }

}
