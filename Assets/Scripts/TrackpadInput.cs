using System;
using UnityEngine;

public class TrackpadInput : MonoBehaviour
{
    static TrackpadInput instance;
    TrackpadPluginHandle pluginHandle;

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

    public bool IsAvailable => pluginHandle?.IsAvailable == true;
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

            if (pluginHandle == null || pluginHandle.IsInvalid)
            {
                Debug.LogWarning("TrackpadInput: Failed to initialize. Check Input Monitoring permissions in System Settings.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"TrackpadInput: Failed to load native plugin: {e.Message}");
        }
    }

    void OnDestroy()
    {
        if (pluginHandle != null && !pluginHandle.IsInvalid)
        {
            pluginHandle.Dispose();
            pluginHandle = null;
        }
    }
}