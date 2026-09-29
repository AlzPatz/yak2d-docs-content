using System.Numerics;
using Yak2D;

namespace YakRun;

public class Game : IApplication
{
    #region fields
    // Game state (plain C# objects, these survive a graphics device reset)
    private Level _level;
    private Yak _yak;
    private Background _background;
    private Vector2 _lastSafePosition;
    private Vector2 _cameraFocus;
    private int _starsCollected;
    private bool _finished;
    private float _time;

    // yak2D resources (created in CreateResources)
    private ITexture _yakTexture;
    private ITexture _grassTexture;
    private ITexture _soilTexture;

    private IDrawStage _backgroundStage;
    private IDrawStage _levelStage;
    private IDrawStage _hudStage;

    private ICamera2D _camera;
    private ICamera2D _backgroundCamera;
    private ICamera2D _hudCamera;
    #endregion

    #region startup
    public StartupConfig Configure()
    {
        return StartupConfig.Default(960, 540, "Yak Run", false);
    }

    public void OnStartup()
    {
        _level = new Level();
        _background = new Background();
        Restart();
    }

    private void Restart()
    {
        _level.Reset();
        _yak = new Yak(_level.Start);
        _lastSafePosition = _level.Start;
        _cameraFocus = _level.Start;
        _starsCollected = 0;
        _finished = false;
        _time = 0.0f;
    }
    #endregion

    #region resources
    public bool CreateResources(IServices yak)
    {
        _yakTexture = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);
        _grassTexture = yak.Surfaces.LoadTexture("grass", AssetSourceEnum.Embedded);
        _soilTexture = yak.Surfaces.LoadTexture("soil", AssetSourceEnum.Embedded);

        // Three draw stages, so each group of drawing can be viewed through its own camera
        _backgroundStage = yak.Stages.CreateDrawStage();
        _levelStage = yak.Stages.CreateDrawStage();
        _hudStage = yak.Stages.CreateDrawStage();

        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _backgroundCamera = yak.Cameras.CreateCamera2D(960, 540);
        _hudCamera = yak.Cameras.CreateCamera2D(960, 540);

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

        if (input.WasKeyPressedThisFrame(KeyCode.R))
        {
            Restart();
        }

        var (move, jump) = ReadControls(input);

        if (_finished)
        {
            move = 0.0f;
            jump = false;
        }
        else
        {
            _time += secondsSinceLastUpdate;
        }

        _yak.Update(secondsSinceLastUpdate, move, jump, _level.Platforms);

        if (_yak.OnGround)
        {
            _lastSafePosition = _yak.Position;
        }

        CollectStars();

        if (!_finished && _yak.Position.X >= _level.Flag.X)
        {
            _finished = true;
        }

        if (_yak.Position.Y < Level.FallLimit)
        {
            // Fell off the level: back to the last place the yak was standing
            _yak = new Yak(_lastSafePosition);
        }

        return true;
    }

    private static (float move, bool jump) ReadControls(IInput input)
    {
        var move = 0.0f;
        if (input.IsKeyCurrentlyPressed(KeyCode.Left) || input.IsKeyCurrentlyPressed(KeyCode.A)) move -= 1.0f;
        if (input.IsKeyCurrentlyPressed(KeyCode.Right) || input.IsKeyCurrentlyPressed(KeyCode.D)) move += 1.0f;

        var jump = input.WasKeyPressedThisFrame(KeyCode.Space) ||
                   input.WasKeyPressedThisFrame(KeyCode.Up) ||
                   input.WasKeyPressedThisFrame(KeyCode.W);

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

    private void CollectStars()
    {
        foreach (var star in _level.Stars.Where(s => !s.Collected))
        {
            if (Vector2.Distance(star.Position, _yak.Position) < 50.0f)
            {
                star.Collected = true;
                _starsCollected++;
            }
        }
    }
    #endregion

    #region predrawing
    public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        // Ease the camera towards the yak, looking a little ahead in the direction of travel
        var target = _yak.Position + new Vector2(0.15f * _yak.Velocity.X, 60.0f);
        target.Y = Math.Clamp(target.Y, -80.0f, 200.0f);
        _cameraFocus = Vector2.Lerp(_cameraFocus, target, 1.0f - MathF.Exp(-6.0f * secondsSinceLastDraw));

        yak.Cameras.SetCamera2DFocusAndZoom(_camera, _cameraFocus, 1.0f);

        // The background camera moves less, so the scenery seems further away
        yak.Cameras.SetCamera2DFocusAndZoom(_backgroundCamera, _cameraFocus * Background.ParallaxFactor, 1.0f);
    }
    #endregion

    #region drawing
    public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _background.Draw(draw, _backgroundStage);
        _level.Draw(draw, _levelStage, _grassTexture, _soilTexture, _time);
        _yak.Draw(draw, _levelStage, _yakTexture);
        DrawHud(draw);
    }

    private void DrawHud(IDrawing draw)
    {
        // HUD text is in Screen space: (0,0) is the middle of the screen, +Y is up, and the
        // virtual screen is 960 x 540. A string's position is the top of the text
        var stars = $"Stars {_starsCollected} / {_level.Stars.Count}";
        draw.DrawString(_hudStage, CoordinateSpace.Screen, stars, Colour.White, 28.0f,
                        new Vector2(-460.0f, 255.0f), TextJustify.Left, 0.5f, 0);

        draw.DrawString(_hudStage, CoordinateSpace.Screen, $"{_time:0.0}s", Colour.White, 28.0f,
                        new Vector2(460.0f, 255.0f), TextJustify.Right, 0.5f, 0);

        if (_time < 4.0f && !_finished)
        {
            draw.DrawString(_hudStage, CoordinateSpace.Screen, "Arrows / A D to run, Space to jump", Colour.White, 22.0f,
                            new Vector2(0.0f, 150.0f), TextJustify.Centre, 0.5f, 0);
        }

        if (_finished)
        {
            draw.Helpers.DrawColouredQuad(_hudStage, CoordinateSpace.Screen, new Colour(0.0f, 0.0f, 0.0f, 0.5f),
                                          new Vector2(0.0f, 20.0f), 620.0f, 150.0f, 0.6f);
            draw.DrawString(_hudStage, CoordinateSpace.Screen, "You made it!", Colour.Gold, 56.0f,
                            new Vector2(0.0f, 80.0f), TextJustify.Centre, 0.5f, 0);
            draw.DrawString(_hudStage, CoordinateSpace.Screen, $"{_starsCollected} stars in {_time:0.0} seconds. R to run again",
                            Colour.White, 24.0f, new Vector2(0.0f, 0.0f), TextJustify.Centre, 0.5f, 0);
        }
    }
    #endregion

    #region rendering
    public void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        // Each draw stage is drawn through its own camera. Depth is cleared in between,
        // so each stage is drawn completely on top of the one before
        q.ClearDepth(windowRenderTarget);
        q.Draw(_backgroundStage, _backgroundCamera, windowRenderTarget);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_levelStage, _camera, windowRenderTarget);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_hudStage, _hudCamera, windowRenderTarget);
    }
    #endregion

    #region shutdown
    public void Shutdown() { }
    #endregion
}
