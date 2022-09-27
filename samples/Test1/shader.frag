#version 330 core

in vec2 fUV;
in vec4 fColor;

uniform sampler2D uTex;

out vec4 FragColor;

void main() {
	FragColor = fColor;
}