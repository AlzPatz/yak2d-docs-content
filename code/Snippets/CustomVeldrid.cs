using NeoVeldrid;
using NeoVeldrid.Sdl2;
using NeoVeldrid.Utilities;
using Yak2D;

// Both Yak2D and NeoVeldrid define some types with the same names (e.g. MouseButton),
// so files that use both usually need an alias or two
using Colour = Yak2D.Colour;
using KeyCode = Yak2D.KeyCode;

namespace Snippets;

// Guide: Custom Veldrid stages (articles/customveldrid.md)

#region custom-veldrid-stage
// The smallest useful custom stage: clears its target to a colour that cycles over time.
// Real stages create pipelines, buffers and shaders in Initialise() and draw in Render()
public class PulseStage : CustomVeldridBase
{
    private float _time;

    public override void Initialise(GraphicsDevice device, Sdl2Window window, DisposeCollectorResourceFactory factory)
    {
        // Create any NeoVeldrid resources here. Ones made with 'factory' are disposed of automatically
    }

    public override void Update(float timeStepSeconds, InputSnapshot inputSnapshot)
    {
        _time += timeStepSeconds; // called once per yak2D update step
    }

    public override void Render(CommandList cl, GraphicsDevice device,
                                ResourceSet texture0, ResourceSet texture1, ResourceSet texture2, ResourceSet texture3,
                                Framebuffer framebufferTarget)
    {
        // Record commands into yak2D's command list. The textures passed to q.CustomVeldrid() arrive as
        // ResourceSets (texture at binding 0, sampler at binding 1), and the target as a Framebuffer
        var pulse = 0.5f + (0.5f * MathF.Sin(_time * 2.0f));
        cl.SetFramebuffer(framebufferTarget);
        cl.ClearColorTarget(0, new RgbaFloat(0.2f, 0.2f * pulse, 0.6f * pulse, 1.0f));
    }

    public override void DisposeOfResources()
    {
        // Dispose of anything not created through the factory
    }
}
#endregion

public class CustomVeldridExample : Example
{
    private ICustomVeldridStage _pulse;
    private IRenderTarget _target;

    #region custom-veldrid-use
    public override bool CreateResources(IServices yak)
    {
        _pulse = yak.Stages.CreateCustomVeldridStage(new PulseStage());
        _target = yak.Surfaces.CreateRenderTarget(960, 540);
        return true;
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.CustomVeldrid(_pulse, null, null, null, null, _target);
        q.Copy(_target, windowRenderTarget);
    }
    #endregion
}
