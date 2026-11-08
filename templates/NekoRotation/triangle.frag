#version 450

layout(set = 1, binding = 1) uniform sampler2D tex0;

layout(location = 0) in vec3 fragColor;
layout(location = 1) in vec2 fragUV;
layout(location = 0) out vec4 outColor;

void main() {
    outColor = vec4(fragColor, 1.0) * texture(tex0, fragUV);
}