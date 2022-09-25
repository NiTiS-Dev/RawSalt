#version 330 core

layout (location = 0) in vec3 vPos;
layout (location = 1) in vec3 vColor;

uniform mat4 uMat;

out vec2 fUV;
out vec3 fColor;

void main() {
	gl_Position = vec4(vPos, 1) * uMat;
	fUV = vec2(1);
	fColor = vColor;
}