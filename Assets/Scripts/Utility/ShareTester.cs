using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareTester : MonoBehaviour
{
    public void ShareText()
    {
        SharePlugin.ShareText("Send to Jessica via...", "Hallo Jessica, kijk mijn plugin werkt!");
    }

    public void ShareImage()
    {
        Application.CaptureScreenshot("Screenshot.png");
        Debug.Log("Saved screenshot to: " + Application.persistentDataPath + "/Screenshot.png");

        SharePlugin.ShareImage("Send image...", Application.persistentDataPath + "/Screenshot.png", "png");
    }
}
