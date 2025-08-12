#!/bin/bash

# Build script for TrackpadPlugin

PLUGIN_NAME="TrackpadPlugin"
SOURCE_FILES="TrackpadPlugin.mm MultitouchSupport.mm"
OUTPUT_FILE="$PLUGIN_NAME.dylib"
UNITY_PLUGIN_DIR="../Assets/Plugins/macOS"

echo "Building TrackpadPlugin for macOS..."

# Compile the plugin
clang++ -dynamiclib \
    -std=c++11 \
    -arch arm64 \
    -arch x86_64 \
    -mmacosx-version-min=10.15 \
    -framework Foundation \
    -framework Cocoa \
    -framework MultitouchSupport \
    -F/System/Library/PrivateFrameworks \
    -fPIC \
    -O2 \
    -Wall \
    -o "$OUTPUT_FILE" \
    $SOURCE_FILES

if [ $? -eq 0 ]; then
    echo "Build successful!"
    
    # Create Unity plugin directory if it doesn't exist
    mkdir -p "$UNITY_PLUGIN_DIR"
    
    # Copy to Unity project
    cp "$OUTPUT_FILE" "$UNITY_PLUGIN_DIR/"
    
    echo "Plugin copied to $UNITY_PLUGIN_DIR"
    
    # Check the architecture
    echo ""
    echo "Architecture info:"
    lipo -info "$OUTPUT_FILE"
    
    # Check dependencies
    echo ""
    echo "Dependencies:"
    otool -L "$OUTPUT_FILE" | grep -E "(MultitouchSupport|Foundation|Cocoa)"
else
    echo "Build failed!"
    exit 1
fi