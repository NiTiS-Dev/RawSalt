#version 330 core

layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vUV;

uniform mat4 uMat;

out vec2 fUV;

void main() {
	gl_Position = vec4(vPos, 1) * uMat;
	fUV = vUV;
}