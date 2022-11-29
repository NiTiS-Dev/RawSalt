#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vUV;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat4 uMVP;

out vec2 fUV;

void main() {
	gl_Position = uMVP * vec4(vPos, 1.0);
	fUV = vUV;
}