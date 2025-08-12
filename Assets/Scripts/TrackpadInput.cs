using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TrackpadInput : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TouchPoint
    {
        public int touchID;
        public float normalizedX;
        public float normalizedY;
        public float force;
        public float majorRadius;
        public float minorRadius;
        public int phase;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchData
    {
        public int touchCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public TouchPoint[] touches;
    }

    public enum TouchPhase
    {
        Began = 0,
        Moved = 1,
        Stationary = 2,
        Ended = 3,
        Cancelled = 4
    }

    #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    const string PluginName = "TrackpadPlugin";

    [DllImport(PluginName)]
    static extern int TrackpadPlugin_Initialize();

    [DllImport(PluginName)]
    static extern void TrackpadPlugin_Shutdown();

    [DllImport(PluginName)]
    static extern void TrackpadPlugin_GetTouchData(ref TouchData data);

    [DllImport(PluginName)]
    static extern int TrackpadPlugin_IsAvailable();
    #endif

    static TrackpadInput instance;
    TouchData currentTouchData;
    bool isInitialized;

    public static TrackpadInput Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("TrackpadInput");
                instance = go.AddComponent<TrackpadInput>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public bool IsAvailable => isInitialized;
    public int TouchCount => currentTouchData.touchCount;
    public TouchData CurrentTouchData => currentTouchData;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        Initialize();
    }

    void Initialize()
    {
        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        try
        {
            currentTouchData = new TouchData { touches = new TouchPoint[10] };
            
            var result = TrackpadPlugin_Initialize();
            isInitialized = result == 1;
            
            if (!isInitialized)
            {
                Debug.LogWarning("TrackpadInput: Failed to initialize. Check Input Monitoring permissions in System Settings.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"TrackpadInput: Failed to load native plugin: {e.Message}");
            isInitialized = false;
        }
        #else
        isInitialized = false;
        #endif
    }

    void Update()
    {
        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        if (!isInitialized) return;

        try
        {
            TrackpadPlugin_GetTouchData(ref currentTouchData);
        }
        catch (Exception e)
        {
            Debug.LogError($"TrackpadInput: Error getting touch data: {e.Message}");
            isInitialized = false;
        }
        #endif
    }

    void OnDestroy()
    {
        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        if (isInitialized)
        {
            try
            {
                TrackpadPlugin_Shutdown();
            }
            catch (Exception e)
            {
                Debug.LogError($"TrackpadInput: Error during shutdown: {e.Message}");
            }
        }
        #endif
    }

    void OnApplicationQuit()
    {
        OnDestroy();
    }

    public TouchPoint? GetTouch(int index)
    {
        if (index < 0 || index >= currentTouchData.touchCount)
            return null;
        return currentTouchData.touches[index];
    }

    public Vector2? GetTouchPosition(int index)
    {
        var touch = GetTouch(index);
        if (touch.HasValue)
            return new Vector2(touch.Value.normalizedX, touch.Value.normalizedY);
        return null;
    }

    public TouchPoint? GetTouchByID(int touchID)
    {
        for (int i = 0; i < currentTouchData.touchCount; i++)
        {
            if (currentTouchData.touches[i].touchID == touchID)
                return currentTouchData.touches[i];
        }
        return null;
    }

    public Vector2 GetPrimaryTouchPosition()
    {
        if (currentTouchData.touchCount > 0)
            return new Vector2(currentTouchData.touches[0].normalizedX, 
                             currentTouchData.touches[0].normalizedY);
        return Vector2.zero;
    }

    public float GetPrimaryTouchForce()
    {
        if (currentTouchData.touchCount > 0)
            return currentTouchData.touches[0].force;
        return 0f;
    }
}