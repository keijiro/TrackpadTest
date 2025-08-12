using System;
using UnityEngine;

public class TrackpadInput : MonoBehaviour
{
    public enum TouchPhase
    {
        Began = 0,
        Moved = 1,
        Stationary = 2,
        Ended = 3,
        Cancelled = 4
    }

    static TrackpadInput instance;
    TrackpadPluginHandle pluginHandle;
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

    public bool IsAvailable => isInitialized && pluginHandle != null && pluginHandle.IsAvailable;
    public int TouchCount => IsAvailable ? pluginHandle.GetTouches().Length : 0;
    public ReadOnlySpan<TrackpadPlugin.TouchPoint> CurrentTouches => IsAvailable ? pluginHandle.GetTouches() : ReadOnlySpan<TrackpadPlugin.TouchPoint>.Empty;

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
        try
        {
            pluginHandle = TrackpadPluginHandle.Create();
            isInitialized = pluginHandle != null && !pluginHandle.IsInvalid;

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
    }

    void Update()
    {
        // No need to update anything here since we're getting touches on demand
    }

    void OnDestroy()
    {
        if (pluginHandle != null && !pluginHandle.IsInvalid)
        {
            pluginHandle.Dispose();
            pluginHandle = null;
        }
    }

    void OnApplicationQuit()
    {
        OnDestroy();
    }

    public TrackpadPlugin.TouchPoint? GetTouch(int index)
    {
        if (!IsAvailable) return null;
        var touches = pluginHandle.GetTouches();
        if (index < 0 || index >= touches.Length)
            return null;
        return touches[index];
    }

    public Vector2? GetTouchPosition(int index)
    {
        var touch = GetTouch(index);
        if (touch.HasValue)
            return new Vector2(touch.Value.normalizedX, touch.Value.normalizedY);
        return null;
    }

    public TrackpadPlugin.TouchPoint? GetTouchByID(int touchID)
    {
        if (!IsAvailable) return null;
        var touches = pluginHandle.GetTouches();
        foreach (var touch in touches)
        {
            if (touch.touchID == touchID)
                return touch;
        }
        return null;
    }

    public Vector2 GetPrimaryTouchPosition()
    {
        if (!IsAvailable) return Vector2.zero;
        var touches = pluginHandle.GetTouches();
        if (touches.Length > 0)
            return new Vector2(touches[0].normalizedX, touches[0].normalizedY);
        return Vector2.zero;
    }

    public float GetPrimaryTouchForce()
    {
        if (!IsAvailable) return 0f;
        var touches = pluginHandle.GetTouches();
        if (touches.Length > 0)
            return touches[0].force;
        return 0f;
    }
}