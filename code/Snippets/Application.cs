using System.Numerics;
using Yak2D;

namespace Snippets;

// Guides: Configuration, Lifecycle, Resources, Input, Window (several articles)

public class FullConfiguration : Example
{
    #region full-config
    public override StartupConfig Configure()
    {
        return new StartupConfig
        {
            // Window
            WindowTitle = "My Game",
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowPositionX = 100,
            WindowPositionY = 100,
            WindowState = DisplayState.Normal,
            WindowIsResizable = true,

            // Graphics
            PreferredGraphicsApi = GraphicsApi.SystemDefault,
            AvoidVulkanWherePossible = false,
            SyncToVerticalBlank = true,
            AutoClearMainWindowColourEachFrame = true,
            AutoClearMainWindowDepthEachFrame = true,

            // Timing
            UpdatePeriodType = UpdatePeriod.Fixed,
            FixedOrSmallestUpdateTimeStepInSeconds = 1.0f / 120.0f,
            ProcessFractionalUpdatesBeforeDraw = true,
            RequireAtleastOneUpdatePerDraw = true,
            FpsCalculationUpdatePeriod = 1.0f,

            // Asset folders
            TextureFolderRootName = "Textures",
            FontFolder = "Fonts",
        };
    }
    #endregion

    private IDrawStage _drawStage;
    private ICamera2D _camera;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(1280, 720);
        return true;
    }

    #region fps
    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.DrawString(_drawStage, CoordinateSpace.Screen, $"Update {fps.UpdateFPS:0}/s   Draw {fps.DrawFPS:0}/s",
                        Colour.White, 24.0f, new Vector2(-620.0f, 340.0f), TextJustify.Left, 0.5f, 0);
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class ResourceRecreation : IApplication
{
    #region recreation
    private ITexture _player;
    private IDrawStage _drawStage;
    private ICamera2D _camera;

    // Game state lives in ordinary fields and is created once
    private Vector2 _playerPosition;

    public void OnStartup()
    {
        _playerPosition = Vector2.Zero;
    }

    // yak2D resources are all created here, so this method can simply be called again
    public bool CreateResources(IServices yak)
    {
        _player = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        return true;
    }

    public void ProcessMessage(FrameworkMessage msg, IServices yak)
    {
        switch (msg)
        {
            case FrameworkMessage.GraphicsDeviceRecreated:
                // Every texture, surface, stage, camera, viewport and font has gone: make them again
                CreateResources(yak);
                break;

            case FrameworkMessage.SwapChainFramebufferReCreated:
                // The window's render target changed (e.g. the window was resized). The one passed to
                // Rendering() is always current; only re-fetch it if you stored it yourself
                break;
        }
    }
    #endregion

    public StartupConfig Configure() => StartupConfig.Default(960, 540, "Resource recreation", false);
    public bool Update(IServices yak, float secondsSinceLastUpdate) => !yak.Input.WasKeyPressedThisFrame(KeyCode.Escape);
    public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastupdate) { }
    public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.World, _player, Colour.White, _playerPosition, 168.0f, 150.0f, 0.5f);
    }
    public void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
    public void Shutdown() { }
}

public class InputExample : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private Vector2 _position;
    private readonly List<Vector2> _clicks = new List<Vector2>();
    private string _lastKey = "";

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        return true;
    }

    #region input-update
    public override bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        var input = yak.Input;
        const float speed = 300.0f;

        // Held keys: true for every update while the key is down. Good for movement
        var direction = Vector2.Zero;
        if (input.IsKeyCurrentlyPressed(KeyCode.Left)) direction.X -= 1.0f;
        if (input.IsKeyCurrentlyPressed(KeyCode.Right)) direction.X += 1.0f;
        if (input.IsKeyCurrentlyPressed(KeyCode.Up)) direction.Y += 1.0f;
        if (input.IsKeyCurrentlyPressed(KeyCode.Down)) direction.Y -= 1.0f;

        // Gamepads: ids of the connected pads, then axes (-1 to 1) and buttons
        foreach (var id in input.ConnectedGamepadIds())
        {
            var stick = new Vector2(input.GamepadAxisValue(id, GamepadAxis.LeftX), -input.GamepadAxisValue(id, GamepadAxis.LeftY));
            if (stick.Length() > 0.2f)
            {
                direction = stick; // SDL reports stick Y as positive downwards, hence the minus above
            }
            if (input.WasGamepadButtonPressedThisFrame(id, GamepadButton.B))
            {
                _clicks.Clear();
            }
        }

        _position += direction * speed * secondsSinceLastUpdate;

        // Pressed / released "this frame": true for exactly one update. Good for actions
        if (input.WasKeyPressedThisFrame(KeyCode.Space))
        {
            _clicks.Clear();
        }

        foreach (var key in input.KeysPressedThisFrame())
        {
            _lastKey = key.ToString();
        }

        // The mouse position is in window pixels, from the top-left corner
        if (input.WasMousePressedThisFrame(MouseButton.Left))
        {
            _clicks.Add(yak.Helpers.CoordinateTransforms.ScreenFromWindow(input.MousePosition, _camera).Position);
        }

        return !input.WasKeyPressedThisFrame(KeyCode.Escape);
    }
    #endregion

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.Helpers.DrawColouredPoly(_drawStage, CoordinateSpace.Screen, Colour.Orange, _position, 32, 30.0f, 0.5f);
        foreach (var c in _clicks)
        {
            draw.Helpers.DrawColouredPoly(_drawStage, CoordinateSpace.Screen, Colour.LightBlue, c, 4, 10.0f, 0.5f);
        }
        draw.DrawString(_drawStage, CoordinateSpace.Screen, $"Arrows / stick to move, click to mark, Space to clear. Last key: {_lastKey}",
                        Colour.White, 20.0f, new Vector2(0.0f, 250.0f), TextJustify.Centre, 0.5f, 0);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class WindowExample : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;

    public override StartupConfig Configure()
    {
        var config = base.Configure();
        config.WindowIsResizable = true;
        return config;
    }

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        return true;
    }

    #region window-update
    public override bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        var display = yak.Display;
        var input = yak.Input;

        if (input.WasKeyPressedThisFrame(KeyCode.F))
        {
            var fullscreen = display.DisplayState == DisplayState.BorderlessFullScreen;
            display.SetDisplayState(fullscreen ? DisplayState.Normal : DisplayState.BorderlessFullScreen);
        }

        if (input.WasKeyPressedThisFrame(KeyCode.Number1)) display.SetWindowResolution(960, 540);
        if (input.WasKeyPressedThisFrame(KeyCode.Number2)) display.SetWindowResolution(1280, 720);

        if (input.WasKeyPressedThisFrame(KeyCode.V))
        {
            _vsync = !_vsync;
            display.SetVsync(_vsync);
        }

        if (input.WasKeyPressedThisFrame(KeyCode.G))
        {
            // Switching graphics API recreates the graphics device: expect GraphicsDeviceRecreated
            var next = yak.Backend.GraphicsApi == GraphicsApi.Vulkan ? GraphicsApi.OpenGL : GraphicsApi.Vulkan;
            if (yak.Backend.IsGraphicsApiSupported(next))
            {
                yak.Backend.SetGraphicsApi(next);
            }
        }

        display.SetWindowTitle($"{display.WindowResolutionWidth} x {display.WindowResolutionHeight} on {yak.Backend.GraphicsApi}");
        return !input.WasKeyPressedThisFrame(KeyCode.Escape);
    }

    private bool _vsync = true;
    #endregion

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.DrawString(_drawStage, CoordinateSpace.Screen, "F fullscreen, 1/2 window size, V vsync, G switch graphics API",
                        Colour.White, 22.0f, new Vector2(0.0f, 20.0f), TextJustify.Centre, 0.5f, 0);
        draw.DrawString(_drawStage, CoordinateSpace.Screen, $"{fps.DrawFPS:0} frames per second",
                        Colour.LightGray, 22.0f, new Vector2(0.0f, -20.0f), TextJustify.Centre, 0.5f, 0);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class AssetLoading : Example
{
    private ITexture _embedded;
    private IFont _font;
    private IDrawStage _drawStage;
    private ICamera2D _camera;

    #region asset-loading
    public override bool CreateResources(IServices yak)
    {
        // Embedded: Textures/yak.png compiled into the assembly (<EmbeddedResource Include="Textures\**" />).
        // The name has no folder and no extension; sub-folders are written with '/' (e.g. "characters/yak")
        _embedded = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);

        // Other image formats say so
        //   yak.Surfaces.LoadTexture("background", AssetSourceEnum.Embedded, ImageFormat.JPG);

        // From a file: Textures/yak.png relative to the working directory (see the Assets guide)
        //   yak.Surfaces.LoadTexture("yak", AssetSourceEnum.File);

        // From any stream, e.g. a download or your own archive format
        //   using var stream = File.OpenRead("somewhere/else/yak.png");
        //   yak.Surfaces.LoadTexture(stream);

        // Fonts: Fonts/snappy_38.fnt, Fonts/snappy_64.fnt and their page images
        _font = yak.Fonts.LoadFont("snappy", AssetSourceEnum.Embedded);

        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        return true;
    }
    #endregion

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _embedded, Colour.White, new Vector2(0.0f, 40.0f), 168.0f, 150.0f, 0.5f);
        draw.DrawString(_drawStage, CoordinateSpace.Screen, "Loaded!", Colour.Orange, 48.0f, new Vector2(0.0f, -60.0f), TextJustify.Centre, 0.5f, 0, _font);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}
