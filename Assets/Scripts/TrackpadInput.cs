using System;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class TrackpadInput : MonoBehaviour
{
    TrackpadPluginHandle _pluginHandle;

    public bool IsAvailable
      => _pluginHandle?.IsAvailable == true;

    public ReadOnlySpan<TrackpadPlugin.TouchPoint> CurrentTouches
      => IsAvailable ? _pluginHandle.GetTouches() :
                       ReadOnlySpan<TrackpadPlugin.TouchPoint>.Empty;

    void Awake()
    {
        var trackpadInputs = FindObjectsByType<TrackpadInput>(FindObjectsSortMode.None);
        Assert.IsTrue(trackpadInputs.Length == 1,
                      "Multiple TrackpadInput components found in the scene.");

        try
        {
            _pluginHandle = TrackpadPluginHandle.Create();

            if (_pluginHandle == null || _pluginHandle.IsInvalid)
                Debug.LogWarning("TrackpadInput: Failed to initialize.");
        }
        catch (Exception e)
        {
            Debug.LogError($"TrackpadInput: {e.Message}");
        }
    }

    void OnDestroy()
    {
        if (!_pluginHandle?.IsInvalid ?? false)
        {
            _pluginHandle.Dispose();
            _pluginHandle = null;
        }
    }
}