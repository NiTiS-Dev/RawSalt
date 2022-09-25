#version 330 core

in vec2 fUV;
in vec3 fColor;

uniform sampler2D uTex;

out vec4 FragColor;

void main() {
	FragColor = vec4(fColor, fColor.x);
}