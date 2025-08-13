using System;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class TrackpadInput : MonoBehaviour
{
    TrackpadPluginHandle pluginHandle;

    public bool IsAvailable
      => pluginHandle?.IsAvailable == true;

    public ReadOnlySpan<TrackpadPlugin.TouchPoint> CurrentTouches
      => IsAvailable ? pluginHandle.GetTouches() :
                       ReadOnlySpan<TrackpadPlugin.TouchPoint>.Empty;

    void Awake()
    {
        var trackpadInputs = FindObjectsByType<TrackpadInput>(FindObjectsSortMode.None);
        Assert.IsTrue(trackpadInputs.Length == 1,
                      "Multiple TrackpadInput components found in the scene.");

        try
        {
            pluginHandle = TrackpadPluginHandle.Create();

            if (pluginHandle == null || pluginHandle.IsInvalid)
                Debug.LogWarning("TrackpadInput: Failed to initialize.");
        }
        catch (Exception e)
        {
            Debug.LogError($"TrackpadInput: {e.Message}");
        }
    }

    void OnDestroy()
    {
        if (!pluginHandle?.IsInvalid ?? false)
        {
            pluginHandle.Dispose();
            pluginHandle = null;
        }
    }
}