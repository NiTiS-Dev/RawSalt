#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vUV;

uniform mat4 uMat; // matrix
uniform float uTime; // time until app staring
uniform vec3 uCamPos; // camera position
uniform ivec2 uWindowSize; // window size

out vec2 fUV;

void main() {
	gl_Position = uMat * vec4(vPos, 1);
	fUV = vUV;
}