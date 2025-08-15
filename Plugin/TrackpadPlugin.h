#ifndef TRACKPAD_PLUGIN_H
#define TRACKPAD_PLUGIN_H

#ifdef __cplusplus
extern "C" {
#endif

typedef struct {
    int touchID;
    float normalizedX;
    float normalizedY;
    float force;
    float majorRadius;
    float minorRadius;
} TouchPoint;

typedef struct {
    int touchCount;
    TouchPoint touches[32]; // Max 32 simultaneous touches
} TouchData;

// Initialize the trackpad monitoring
int TrackpadPlugin_Initialize(void);

// Shutdown the trackpad monitoring
void TrackpadPlugin_Shutdown(void);

// Get current touch data
void TrackpadPlugin_GetTouchData(TouchData* data);

// Check if trackpad is available
int TrackpadPlugin_IsAvailable(void);

#ifdef __cplusplus
}
#endif

#endif // TRACKPAD_PLUGIN_H