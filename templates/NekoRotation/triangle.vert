#version 450

layout(std140, row_major, set = 0, binding = 0) uniform Camera {
    mat4 view;
    mat4 projection;
} camera;

layout(std140, row_major, set = 1, binding = 0) uniform Draw {
    mat4 model;
} draw;

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inColor;
layout(location = 2) in vec2 inUV;

layout(location = 0) out vec3 fragColor;
layout(location = 1) out vec2 fragUV;

void main() {
	gl_Position = vec4(inPosition, 1.0) * draw.model * camera.view * camera.projection;
	fragColor = inColor;
    fragUV = inUV;
}