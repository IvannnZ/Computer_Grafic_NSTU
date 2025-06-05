#version 330 core

in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

uniform vec3 lightDir = normalize(vec3(-1.0, -1.0, -1.0));
uniform vec3 objectColor = vec3(1.0, 0.5, 0.3);

void main()
{
    vec3 norm = normalize(Normal);
    float diff = max(dot(norm, -lightDir), 0.0);
    vec3 result = diff * objectColor;
    FragColor = vec4(result, 1.0);
}
