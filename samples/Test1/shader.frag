#version 330 core

in vec2 fUV;

uniform sampler2D uTex0;
uniform vec4 uColor;

out vec4 FragColor;

void main() {
	vec4 tex = texture(uTex0, fUV);
	tex = tex * uColor;
	if (tex == vec4(0))
	{
		FragColor = vec4(fUV, 1, 1);
	}
	else
	{
		FragColor = tex;
	}
}