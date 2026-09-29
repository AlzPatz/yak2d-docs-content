using System.Numerics;
using Yak2D;

namespace YakRun;

public class Game : IApplication
{
    #region fields
    // Game state
    private Yak _yak;

    // The solid things the yak can stand on: the ground and two blocks
    private readonly List<Box> _platforms = new List<Box>
    {
        new Box(-480.0f, -600.0f, 960.0f, 525.0f),
        new Box(120.0f, -75.0f, 160.0f, 70.0f),
        new Box(-330.0f, -75.0f, 110.0f, 150.0f),
    };

    // yak2D resources
    private ITexture _yakTexture;
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    #endregion

    #region startup
    public StartupConfig Configure()
    {
        return StartupConfig.Default(960, 540, "Yak Run", false);
    }

    public void OnStartup()
    {
        _yak = new Yak(new Vector2(-100.0f, -45.0f));
    }
    #endregion

    #region resources
    public bool CreateResources(IServices yak)
    {
        _yakTexture = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        return true;
    }

    public void ProcessMessage(FrameworkMessage msg, IServices yak)
    {
        if (msg == FrameworkMessage.GraphicsDeviceRecreated)
        {
            CreateResources(yak);
        }
    }
    #endregion

    #region update
    public bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        var input = yak.Input;

        if (input.WasKeyPressedThisFrame(KeyCode.Escape))
        {
            return false;
        }

        var (move, jump) = ReadControls(input);

        _yak.Update(secondsSinceLastUpdate, move, jump, _platforms);

        return true;
    }

    private static (float move, bool jump) ReadControls(IInput input)
    {
        var move = 0.0f;
        if (input.IsKeyCurrentlyPressed(KeyCode.Left) || input.IsKeyCurrentlyPressed(KeyCode.A)) move -= 1.0f;
        if (input.IsKeyCurrentlyPressed(KeyCode.Right) || input.IsKeyCurrentlyPressed(KeyCode.D)) move += 1.0f;

        // "Pressed this frame" is only true for the first update after the key goes down,
        // so holding space does not make the yak bounce continuously
        var jump = input.WasKeyPressedThisFrame(KeyCode.Space) ||
                   input.WasKeyPressedThisFrame(KeyCode.Up) ||
                   input.WasKeyPressedThisFrame(KeyCode.W);

        // Any connected gamepad works too: left stick to run, A to jump
        foreach (var id in input.ConnectedGamepadIds())
        {
            var stick = input.GamepadAxisValue(id, GamepadAxis.LeftX);
            if (MathF.Abs(stick) > 0.25f)
            {
                move = stick;
            }
            jump |= input.WasGamepadButtonPressedThisFrame(id, GamepadButton.A);
        }

        return (Math.Clamp(move, -1.0f, 1.0f), jump);
    }

    public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }
    #endregion

    #region drawing
    public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        foreach (var platform in _platforms)
        {
            draw.Helpers.DrawColouredQuad(_drawStage, CoordinateSpace.World, Colour.ForestGreen,
                                          platform.Centre, platform.Width, platform.Height, 0.9f);
        }

        _yak.Draw(draw, _drawStage, _yakTexture);
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
