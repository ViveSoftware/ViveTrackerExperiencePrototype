//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using UnityEngine.Networking;

namespace HTC.UnityPlugin.ViveShare
{
    //-----------------------------------------------------------------------------
    // server/client state definition
    //-----------------------------------------------------------------------------

    public enum ViveNetworkState
    {
        Init,
        Wait_Connect,         // for client only
        Wait_TrackerRegistry, // for client only
        Wait_Ready,
        Wait_GameStart,
        Gaming,
        Wait_Shutdown
    }

    //-----------------------------------------------------------------------------
    // log
    //-----------------------------------------------------------------------------

    public static class ViveShare_Log
    {
        public static void LogError(LogFilter.FilterLevel curLogLevel, string str)
        {
            if (curLogLevel <= LogFilter.FilterLevel.Error)
            {
                Debug.LogError(str);
            }
        }

        public static void LogWarning(LogFilter.FilterLevel curLogLevel, string str)
        {
            if (curLogLevel <= LogFilter.FilterLevel.Warn)
            {
                Debug.LogWarning(str);
            }
        }

        public static void LogInfo(LogFilter.FilterLevel curLogLevel, string str)
        {
            if (curLogLevel <= LogFilter.FilterLevel.Info)
            {
                Debug.Log(str);
            }
        }

        public static void LogDebug(LogFilter.FilterLevel curLogLevel, string str)
        {
            if (curLogLevel <= LogFilter.FilterLevel.Debug)
            {
                Debug.Log(str);
            }
        }
    }

    //-----------------------------------------------------------------------------
    // singleton definition
    //-----------------------------------------------------------------------------

    [DisallowMultipleComponent]
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();
        private static bool applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "[Singleton -- " + typeof(T).ToString() + "]";
                        }

                        DontDestroyOnLoad(_instance.gameObject);
                    }

                    return _instance;
                }
            }
        }

        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }

    public class Singleton<T> where T : new()
    {
        private static T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }

                    return _instance;
                }
            }
        }
    }
}
