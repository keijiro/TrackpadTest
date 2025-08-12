using System;
using System.Runtime.InteropServices;

public static class TrackpadPlugin
{
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
    public unsafe struct TouchData
    {
        public int touchCount;
        public fixed byte touchesBuffer[10 * 32]; // 10 TouchPoints * 32 bytes each

        public Span<TouchPoint> GetTouches()
        {
            fixed (byte* ptr = touchesBuffer)
            {
                return new Span<TouchPoint>(ptr, touchCount);
            }
        }

        public Span<TouchPoint> GetAllTouchSlots()
        {
            fixed (byte* ptr = touchesBuffer)
            {
                return new Span<TouchPoint>(ptr, 10);
            }
        }
    }

    public static int Initialize()
    {
        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        return TrackpadPlugin_Initialize();
        #else
        return 0;
        #endif
    }

    public static void Shutdown()
    {
        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        TrackpadPlugin_Shutdown();
        #endif
    }

    public static void GetTouchData(ref TouchData data)
    {
        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        TrackpadPlugin_GetTouchData(ref data);
        #else
        data.touchCount = 0;
        #endif
    }

    public static bool IsAvailable()
    {
        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        return TrackpadPlugin_IsAvailable() == 1;
        #else
        return false;
        #endif
    }
}