using System.Numerics;
using Yak2D;

namespace Snippets;

// Guide: How yak2D works (articles/overview.md). A complete application in one class.
// Run it with: dotnet run -- OverviewGame

#region overview
public class OverviewGame : IApplication
{
    private ITexture _yak;
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private IRenderTarget _scene;
    private IColourEffectsStage _sepia;

    public StartupConfig Configure() => StartupConfig.Default(960, 540, "Overview", false);

    public void OnStartup() { }

    public bool CreateResources(IServices yak)
    {
        _yak = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _scene = yak.Surfaces.CreateRenderTarget(960, 540);
        _sepia = yak.Stages.CreateColourEffectsStage();
        yak.Stages.SetColourEffectsConfig(_sepia, new ColourEffectConfiguration
        {
            Colourise = 0.8f,
            ColourForSingleColourAndColourise = Colour.Burlywood,
            Opacity = 1.0f
        });
        return true;
    }

    public void ProcessMessage(FrameworkMessage msg, IServices yak)
    {
        if (msg == FrameworkMessage.GraphicsDeviceRecreated)
        {
            CreateResources(yak);
        }
    }

    public bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        return !yak.Input.WasKeyPressedThisFrame(KeyCode.Escape);
    }

    public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }

    public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                        float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.Helpers.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.SkyBlue, Vector2.Zero, 960.0f, 540.0f, 0.9f);
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.World, _yak, Colour.White, Vector2.Zero, 168.0f, 150.0f, 0.5f);
    }

    public void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.Draw(_drawStage, _camera, _scene);                 // draw the scene off-screen...
        q.ColourEffects(_sepia, _scene, windowRenderTarget); // ...then colourise it onto the window
    }

    public void Shutdown() { }
}
#endregion
