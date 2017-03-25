using UnityEngine;

public static class GPSPlugin
{
    #if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject m_AndroidPlugin;
    #endif

    private static AndroidJavaObject GetGPSPlugin()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            if (m_AndroidPlugin != null)
                return m_AndroidPlugin;

            AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            m_AndroidPlugin = new AndroidJavaObject("com.kapistijn.androidgpsplugin.AndroidGPSPlugin", currentActivity);

            return m_AndroidPlugin;
        #else
            return null;
        #endif
    }

    //Methods
    public static double GetLatitude()
    {
        if (GetGPSPlugin() == null)
            return 0.0;

        return GetGPSPlugin().Call<double>("GetLatitude");
    }

    public static double GetLongitude()
    {
        if (GetGPSPlugin() == null)
            return 0.0;

        return GetGPSPlugin().Call<double>("GetLongitude");
    }
}
