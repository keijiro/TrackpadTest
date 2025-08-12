#import <Foundation/Foundation.h>
#import <dlfcn.h>
#include "MultitouchSupport.h"

// Function pointers for MultitouchSupport framework
void* (*MTDeviceCreateDefault)(void) = nullptr;
CFArrayRef (*MTDeviceCreateList)(void) = nullptr;
void (*MTRegisterContactFrameCallback)(MTDeviceRef device, MTContactCallbackFunction callback) = nullptr;
void (*MTUnregisterContactFrameCallback)(MTDeviceRef device, MTContactCallbackFunction callback) = nullptr;
void (*MTDeviceStart)(MTDeviceRef device, int runMode) = nullptr;
void (*MTDeviceStop)(MTDeviceRef device) = nullptr;
void (*MTDeviceRelease)(MTDeviceRef device) = nullptr;

static void* g_multitouchLib = nullptr;

int MultitouchSupport_Initialize(void) {
    if (g_multitouchLib != nullptr) {
        return 1; // Already initialized
    }
    
    // Load MultitouchSupport framework
    g_multitouchLib = dlopen("/System/Library/PrivateFrameworks/MultitouchSupport.framework/MultitouchSupport", RTLD_LAZY);
    if (!g_multitouchLib) {
        return 0;
    }
    
    // Get function pointers
    MTDeviceCreateDefault = (void* (*)(void))dlsym(g_multitouchLib, "MTDeviceCreateDefault");
    MTDeviceCreateList = (CFArrayRef (*)(void))dlsym(g_multitouchLib, "MTDeviceCreateList");
    MTRegisterContactFrameCallback = (void (*)(MTDeviceRef, MTContactCallbackFunction))dlsym(g_multitouchLib, "MTRegisterContactFrameCallback");
    MTUnregisterContactFrameCallback = (void (*)(MTDeviceRef, MTContactCallbackFunction))dlsym(g_multitouchLib, "MTUnregisterContactFrameCallback");
    MTDeviceStart = (void (*)(MTDeviceRef, int))dlsym(g_multitouchLib, "MTDeviceStart");
    MTDeviceStop = (void (*)(MTDeviceRef))dlsym(g_multitouchLib, "MTDeviceStop");
    MTDeviceRelease = (void (*)(MTDeviceRef))dlsym(g_multitouchLib, "MTDeviceRelease");
    
    if (!MTDeviceCreateDefault || !MTRegisterContactFrameCallback || !MTDeviceStart) {
        dlclose(g_multitouchLib);
        g_multitouchLib = nullptr;
        return 0;
    }
    
    return 1;
}

void MultitouchSupport_Shutdown(void) {
    if (g_multitouchLib) {
        dlclose(g_multitouchLib);
        g_multitouchLib = nullptr;
        
        // Reset function pointers
        MTDeviceCreateDefault = nullptr;
        MTDeviceCreateList = nullptr;
        MTRegisterContactFrameCallback = nullptr;
        MTUnregisterContactFrameCallback = nullptr;
        MTDeviceStart = nullptr;
        MTDeviceStop = nullptr;
        MTDeviceRelease = nullptr;
    }
}

MTDeviceRef MultitouchSupport_CreateDevice(void) {
    if (!g_multitouchLib) {
        return nullptr;
    }
    
    MTDeviceRef device = nullptr;
    
    // Try to get device from list first
    if (MTDeviceCreateList) {
        CFArrayRef deviceList = MTDeviceCreateList();
        if (deviceList) {
            CFIndex count = CFArrayGetCount(deviceList);
            if (count > 0) {
                device = (MTDeviceRef)CFArrayGetValueAtIndex(deviceList, 0);
                CFRetain(device);
            }
            CFRelease(deviceList);
        }
    }
    
    // Fallback to default device
    if (!device && MTDeviceCreateDefault) {
        device = MTDeviceCreateDefault();
    }
    
    return device;
}