#ifndef MULTITOUCH_SUPPORT_H
#define MULTITOUCH_SUPPORT_H

#include <CoreFoundation/CoreFoundation.h>

#ifdef __cplusplus
extern "C" {
#endif

// MultitouchSupport private framework types
typedef struct {
    float x;
    float y;
} mtPoint;

typedef struct {
    mtPoint position;
    mtPoint velocity;
} mtReadout;

typedef struct {
    int frame;
    double timestamp;
    int identifier;
    int state;
    int fingerID;
    int handID;
    mtReadout normalized;
    float size;
    int zero1;
    float angle;
    float majorAxis;
    float minorAxis;
    mtReadout absolute;
    int zero2[2];
    float density;
} mtTouch;

typedef void* MTDeviceRef;
typedef int (*MTContactCallbackFunction)(int device, mtTouch* touches, int numTouches, double timestamp, int frame);

// MultitouchSupport API functions
extern void* (*MTDeviceCreateDefault)(void);
extern CFArrayRef (*MTDeviceCreateList)(void);
extern void (*MTRegisterContactFrameCallback)(MTDeviceRef device, MTContactCallbackFunction callback);
extern void (*MTUnregisterContactFrameCallback)(MTDeviceRef device, MTContactCallbackFunction callback);
extern void (*MTDeviceStart)(MTDeviceRef device, int runMode);
extern void (*MTDeviceStop)(MTDeviceRef device);
extern void (*MTDeviceRelease)(MTDeviceRef device);

// Initialization and cleanup
int MultitouchSupport_Initialize(void);
void MultitouchSupport_Shutdown(void);
MTDeviceRef MultitouchSupport_CreateDevice(void);

#ifdef __cplusplus
}
#endif

#endif // MULTITOUCH_SUPPORT_H