using System;
using Microsoft.Win32.SafeHandles;

// Safe handle for the native plugin
public sealed class TrackpadPluginHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    TrackpadPlugin.TouchData touchData;

    TrackpadPluginHandle() : base(true)
    {
        touchData = new TrackpadPlugin.TouchData();
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            TrackpadPlugin.Shutdown();
        }
        return true;
    }

    public static TrackpadPluginHandle Create()
    {
        var result = TrackpadPlugin.Initialize();
        if (result == 1)
        {
            var handle = new TrackpadPluginHandle();
            handle.SetHandle(new IntPtr(1));
            return handle;
        }
        return null;
    }

    public ReadOnlySpan<TrackpadPlugin.TouchPoint> GetTouches()
    {
        if (!IsInvalid)
        {
            TrackpadPlugin.GetTouchData(ref touchData);
            return touchData.GetTouches();
        }
        return ReadOnlySpan<TrackpadPlugin.TouchPoint>.Empty;
    }

    public bool IsAvailable => !IsInvalid && TrackpadPlugin.IsAvailable();
}