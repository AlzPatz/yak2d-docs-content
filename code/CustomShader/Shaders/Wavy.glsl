#version 450

// Input 0: the texture passed to q.CustomShader() as tex0. Uniform description "Texture" -> resource set 0
layout(set = 0, binding = 0) uniform texture2D Texture_Texture;
layout(set = 0, binding = 1) uniform sampler Sampler_Texture;

// Uniform description "Settings" -> resource set 1. Must match the C# struct's layout
layout(set = 1, binding = 0) uniform Settings
{
    float Time;
    float Amount;
    vec2 Pad;
};

// From yak2D's vertex shader: texture coordinates, (0,0) top-left to (1,1) bottom-right
layout(location = 0) in vec2 FTex;

layout(location = 0) out vec4 fragColor;

void main()
{
    // Shift where we read from with a moving sine wave, so the image ripples
    vec2 offset = Amount * vec2(sin(FTex.y * 20.0 + Time * 3.0), cos(FTex.x * 16.0 + Time * 2.0)) * 0.02;
    vec4 colour = texture(sampler2D(Texture_Texture, Sampler_Texture), FTex + offset);

    // Slowly cycle the colours by rotating red, green and blue
    float t = Time * 0.5;
    vec3 cycled = vec3(colour.r * cos(t) + colour.g * sin(t), colour.g, colour.b * cos(t) + colour.r * sin(t));
    fragColor = vec4(mix(colour.rgb, cycled, Amount), colour.a);
}
