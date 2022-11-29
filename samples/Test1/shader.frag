#version 330 core

in vec2 fUV;

uniform sampler2D uTex0;
uniform vec4 uColor = vec4(1);

out vec4 FragColor;

void main() {
	vec4 mixColor = texture(uTex0, fUV);
	mixColor *= uColor;
	FragColor = mixColor;
}