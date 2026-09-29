using System.Runtime.InteropServices;
using Yak2D;

namespace CustomShader;

public class WavyYak : IApplication
{
    #region uniforms
    // The C# side of the shader's "Settings" uniform block. Uniform data must be a multiple of
    // 16 bytes, and match the shader's layout exactly, hence the padding
    [StructLayout(LayoutKind.Sequential)]
    private struct Settings
    {
        public float Time;
        public float Amount;
        public float Pad0;
        public float Pad1;
    }
    #endregion

    private ITexture _yak;
    private ICustomShaderStage _wavy;
    private float _time;
    private float _amount = 1.0f;

    public StartupConfig Configure() => StartupConfig.Default(960, 540, "Custom Shader", false);

    public void OnStartup() { }

    #region create
    public bool CreateResources(IServices yak)
    {
        _yak = yak.Surfaces.LoadTexture("yakphoto", AssetSourceEnum.Embedded);

        _wavy = yak.Stages.CreateCustomShaderStage(
            "Wavy",                        // Shaders/Wavy.glsl
            AssetSourceEnum.Embedded,
            new[]
            {
                new ShaderUniformDescription { Name = "Texture", UniformType = ShaderUniformType.Texture, SizeInBytes = 0 },
                new ShaderUniformDescription { Name = "Settings", UniformType = ShaderUniformType.Data, SizeInBytes = 16 },
            },
            BlendState.Override,           // replace the target's pixels rather than blending with them
            useSpirvCompile: true);        // compile the one .glsl file for whichever graphics API is in use

        return true;
    }
    #endregion

    public void ProcessMessage(FrameworkMessage msg, IServices yak)
    {
        if (msg == FrameworkMessage.GraphicsDeviceRecreated)
        {
            CreateResources(yak);
        }
    }

    #region update
    public bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        // Up and down arrows change the strength of the effect
        if (yak.Input.IsKeyCurrentlyPressed(KeyCode.Up)) _amount = MathF.Min(_amount + secondsSinceLastUpdate, 2.0f);
        if (yak.Input.IsKeyCurrentlyPressed(KeyCode.Down)) _amount = MathF.Max(_amount - secondsSinceLastUpdate, 0.0f);

        return !yak.Input.WasKeyPressedThisFrame(KeyCode.Escape);
    }

    public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;

        // Send this frame's values to the shader
        yak.Stages.SetCustomShaderUniformValues(_wavy, "Settings", new Settings { Time = _time, Amount = _amount });
    }
    #endregion

    public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                        float secondsSinceLastDraw, float secondsSinceLastUpdate) { }

    #region render
    public void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        // Up to four input textures. This shader only reads the first
        q.CustomShader(_wavy, _yak, null, null, null, windowRenderTarget);
    }
    #endregion

    public void Shutdown() { }
}
