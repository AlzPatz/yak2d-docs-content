using System.Numerics;
using Yak2D;

namespace YakRun;

public class Game : IApplication
{
    #region fields
    private ITexture _yakTexture;
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    #endregion

    #region startup
    public StartupConfig Configure()
    {
        return StartupConfig.Default(960, 540, "Yak Run", false);
    }

    public void OnStartup() { }
    #endregion

    #region resources
    public bool CreateResources(IServices yak)
    {
        // "yak" means Textures/yak.png, embedded in this assembly (see YakRun.csproj)
        _yakTexture = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);

        // A draw stage collects and sorts everything we draw each frame
        _drawStage = yak.Stages.CreateDrawStage();

        // A camera with a 960 x 540 "virtual resolution": whatever the window size,
        // it always shows 960 x 540 world units
        _camera = yak.Cameras.CreateCamera2D(960, 540);

        return true;
    }

    public void ProcessMessage(FrameworkMessage msg, IServices yak)
    {
        // All yak2D resources are lost if the graphics device is recreated (e.g. switching graphics API)
        if (msg == FrameworkMessage.GraphicsDeviceRecreated)
        {
            CreateResources(yak);
        }
    }
    #endregion

    #region update
    public bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        return !yak.Input.WasKeyPressedThisFrame(KeyCode.Escape);
    }

    public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }
    #endregion

    #region drawing
    public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        // The yak: a textured rectangle ("quad"), centred on the world origin
        draw.Helpers.DrawTexturedQuad(_drawStage,
                                      CoordinateSpace.World,
                                      _yakTexture,
                                      Colour.White,       // White leaves the texture's colours unchanged
                                      new Vector2(0.0f, -38.0f),
                                      84.0f,              // width
                                      75.0f,              // height
                                      0.5f);              // depth: 0 is at the front, 1 at the back

        // The ground: a plain coloured rectangle, further back (depth 0.9) than the yak
        draw.Helpers.DrawColouredQuad(_drawStage,
                                      CoordinateSpace.World,
                                      Colour.ForestGreen,
                                      new Vector2(0.0f, -175.0f),
                                      960.0f,
                                      200.0f,
                                      0.9f);
    }
    #endregion

    #region rendering
    public void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.SkyBlue);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
    #endregion

    #region shutdown
    public void Shutdown() { }
    #endregion
}
