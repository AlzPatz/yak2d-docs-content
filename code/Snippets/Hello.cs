using System.Numerics;
using Yak2D;

namespace Snippets;

// The example on the documentation home page (index.md)

#region hello
public class Hello : IApplication
{
    private IDrawStage _stage;
    private ICamera2D _camera;

    public StartupConfig Configure() => StartupConfig.Default(960, 540, "Hello, yak2D", false);

    public void OnStartup() { }

    public bool CreateResources(IServices yak)
    {
        _stage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        return true;
    }

    public void ProcessMessage(FrameworkMessage msg, IServices yak)
    {
        if (msg == FrameworkMessage.GraphicsDeviceRecreated) CreateResources(yak);
    }

    public bool Update(IServices yak, float seconds) => !yak.Input.WasKeyPressedThisFrame(KeyCode.Escape);

    public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }

    public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                        float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.Helpers.DrawColouredPoly(_stage, CoordinateSpace.Screen, Colour.Gold, Vector2.Zero, 5, 120.0f, 0.5f);
        draw.DrawString(_stage, CoordinateSpace.Screen, "Hello, yak2D", Colour.White, 48.0f,
                        new Vector2(0.0f, -150.0f), TextJustify.Centre, 0.5f, 0);
    }

    public void Rendering(IRenderQueue q, IRenderTarget window)
    {
        q.ClearColour(window, Colour.DarkSlateBlue);
        q.ClearDepth(window);
        q.Draw(_stage, _camera, window);
    }

    public void Shutdown() { }
}
#endregion
