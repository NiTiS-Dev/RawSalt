# RawSalt Engine

## Graphics

### UV Coordinates
Raw Salt Engine uses top-left corner as texture origin.
Texture coordinates are normalized float values from 0f to 1f, and are not clamped.

## Application Requirements

### Operating system support

| Platform |    Requirements    |
|:--------:|:------------------:|
|  Win32   |    Windows 10+     |
|   Xlib   | Not supported yet. |
| Wayland  | Not supported yet. |
| Android  | Not supported yet. |

### Graphical API support

|   API   |    Requirements    |      Supported OS       |
|:-------:|:------------------:|:-----------------------:|
| Vulkan  |        1.3+        | Windows, Linux, Android |
| DirectX | Not supported yet. |      Only Windows       |
|  Metal  | Not supported yet. |       Only MacOS        |
| OpenGL  |    Not planned.    |                         |