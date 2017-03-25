using UnityEngine;

public static class SharePlugin
{
    #if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject m_AndroidPlugin;
    #endif

    private static AndroidJavaObject GetSharePlugin()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (m_AndroidPlugin != null)
                return m_AndroidPlugin;

            AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            m_AndroidPlugin = new AndroidJavaObject("com.kapistijn.androidshareplugin.AndroidSharePlugin", currentActivity);

            return m_AndroidPlugin;
        #else
            return null;
        #endif
    }

    //Methods
    public static void ShareText(string chooserTitle, string text)
    {
        if (GetSharePlugin() == null)
            return;

        GetSharePlugin().Call("ShareText", chooserTitle, text);
    }

    public static void ShareImage(string chooserTitle, string imagePath, string imageType)
    {
        if (GetSharePlugin() == null)
            return;

        GetSharePlugin().Call("ShareImage", chooserTitle, imagePath, imageType);
    }
}
