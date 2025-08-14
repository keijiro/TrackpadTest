#import <Foundation/Foundation.h>
#import <CoreFoundation/CoreFoundation.h>
#include "TrackpadPlugin.h"
#include "MultitouchSupport.h"
#include <mutex>

// Global state
static MTDeviceRef g_device = nullptr;
static TouchData g_currentTouchData;
static std::mutex g_touchDataMutex;
static CFRunLoopRef g_runLoop = nullptr;
static bool g_isRunning = false;

// Callback function for multitouch events
static int MultitouchCallback(int device, mtTouch* touches, int numTouches, double timestamp, int frame) {
    std::lock_guard<std::mutex> lock(g_touchDataMutex);
    
    g_currentTouchData.touchCount = std::min(numTouches, 32);
    
    for (int i = 0; i < g_currentTouchData.touchCount; i++) {
        TouchPoint& point = g_currentTouchData.touches[i];
        const mtTouch& touch = touches[i];
        
        point.touchID = touch.identifier;
        point.normalizedX = touch.normalized.position.x;
        point.normalizedY = touch.normalized.position.y;
        point.force = touch.density;
        point.majorRadius = touch.majorAxis;
        point.minorRadius = touch.minorAxis;
        
        switch (touch.state) {
            case 1: point.phase = 0; break; // Began
            case 2: point.phase = 1; break; // Moved
            case 3: point.phase = 2; break; // Stationary
            case 4: point.phase = 3; break; // Ended
            case 5: point.phase = 4; break; // Cancelled
            default: point.phase = 1; break;
        }
    }
    
    return 0;
}

int TrackpadPlugin_Initialize(void) {
    if (g_device != nullptr) {
        return 1; // Already initialized
    }
    
    // Initialize MultitouchSupport
    if (!MultitouchSupport_Initialize()) {
        return 0;
    }
    
    // Create device
    g_device = MultitouchSupport_CreateDevice();
    if (!g_device) {
        MultitouchSupport_Shutdown();
        return 0;
    }
    
    // Initialize touch data
    g_currentTouchData.touchCount = 0;
    
    // Register callback and start device
    MTRegisterContactFrameCallback(g_device, MultitouchCallback);
    MTDeviceStart(g_device, 0);
    
    g_isRunning = true;
    
    // Start run loop on background thread
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        g_runLoop = CFRunLoopGetCurrent();
        
        // Keep-alive timer
        CFRunLoopTimerRef timer = CFRunLoopTimerCreate(
            kCFAllocatorDefault,
            CFAbsoluteTimeGetCurrent() + 1.0,
            1.0,
            0, 0,
            [](CFRunLoopTimerRef timer, void* info) {},
            nullptr
        );
        
        CFRunLoopAddTimer(g_runLoop, timer, kCFRunLoopDefaultMode);
        
        while (g_isRunning) {
            CFRunLoopRunInMode(kCFRunLoopDefaultMode, 0.1, false);
        }
        
        CFRunLoopTimerInvalidate(timer);
        CFRelease(timer);
    });
    
    return 1;
}

void TrackpadPlugin_Shutdown(void) {
    if (g_device) {
        g_isRunning = false;
        
        if (MTDeviceStop) {
            MTDeviceStop(g_device);
        }
        if (MTUnregisterContactFrameCallback) {
            MTUnregisterContactFrameCallback(g_device, MultitouchCallback);
        }
        if (MTDeviceRelease) {
            MTDeviceRelease(g_device);
        }
        g_device = nullptr;
        
        MultitouchSupport_Shutdown();
    }
}

void TrackpadPlugin_GetTouchData(TouchData* data) {
    if (!data) return;
    
    std::lock_guard<std::mutex> lock(g_touchDataMutex);
    *data = g_currentTouchData;
}

int TrackpadPlugin_IsAvailable(void) {
    return (g_device != nullptr) ? 1 : 0;
}