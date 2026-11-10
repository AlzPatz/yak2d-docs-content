using System.Numerics;
using Yak2D;

namespace Snippets;

// Guide: Cookbook (articles/cookbook.md)

public class SpriteAnimation : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _coin;
    private float _time;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _coin = yak.Surfaces.LoadTexture("coin", AssetSourceEnum.Embedded);
        return true;
    }

    #region sprite-animation
    private const int FrameCount = 8;          // frames side by side in coin.png
    private const float FramesPerSecond = 12.0f;

    // Draws one frame of a horizontal sprite sheet by choosing which slice of the texture to show
    private void DrawFrame(IDrawingHelpers h, Vector2 position, float size, int frame, bool flip = false)
    {
        var left = frame / (float)FrameCount;
        var right = (frame + 1) / (float)FrameCount;
        if (flip)
        {
            (left, right) = (right, left); // swapping the x texture coordinates mirrors the sprite
        }

        h.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _coin, Colour.White, position, size, size, 0.5f,
                           texcoord_min_x: left, texcoord_max_x: right);
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;
        var frame = (int)(_time * FramesPerSecond) % FrameCount;

        // A row of coins, each started a frame later than the last, so they ripple
        for (var n = 0; n < 7; n++)
        {
            DrawFrame(draw.Helpers, new Vector2(-360.0f + (n * 120.0f), 60.0f), 96.0f, (frame + n) % FrameCount);
        }

        // Every frame of the sheet, for reference
        for (var n = 0; n < FrameCount; n++)
        {
            DrawFrame(draw.Helpers, new Vector2(-350.0f + (n * 100.0f), -140.0f), 64.0f, n);
            draw.DrawString(_drawStage, CoordinateSpace.Screen, n.ToString(), Colour.LightGray, 20.0f,
                            new Vector2(-350.0f + (n * 100.0f), -180.0f), TextJustify.Centre, 0.5f, 0);
        }
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateBlue);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class ScreenShake : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _yak;
    private ITexture _soil;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _yak = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);
        _soil = yak.Surfaces.LoadTexture("soil", AssetSourceEnum.Embedded);
        return true;
    }

    #region screen-shake
    private float _trauma;   // 0 = still, 1 = maximum shake
    private float _time;

    public override bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        if (yak.Input.WasKeyPressedThisFrame(KeyCode.Space))
        {
            _trauma = MathF.Min(1.0f, _trauma + 0.6f); // hits add up, to a limit
        }

        _trauma = MathF.Max(0.0f, _trauma - (1.2f * secondsSinceLastUpdate)); // and wear off over time
        return base.Update(yak, secondsSinceLastUpdate);
    }

    public override void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;

        // Squaring makes small knocks subtle and big ones violent. Sine waves at unrelated frequencies
        // give a smooth, random-looking wobble (use a noise function if you have one)
        var shake = _trauma * _trauma;
        var offset = new Vector2(MathF.Sin(_time * 41.0f) + MathF.Sin(_time * 23.0f),
                                 MathF.Sin(_time * 37.0f) + MathF.Sin(_time * 19.0f)) * (12.0f * shake);
        var angle = 0.05f * shake * MathF.Sin(_time * 29.0f);

        var focus = Vector2.Zero; // wherever the camera would normally look
        yak.Cameras.SetCamera2DFocusZoomAndRotation(_camera, focus + offset, 1.0f, angle);
    }
    #endregion

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.World, _soil, Colour.White, new Vector2(0.0f, -200.0f), 1200.0f, 300.0f, 0.8f,
                                      texcoord_max_x: 1200.0f / 128.0f, texcoord_max_y: 300.0f / 128.0f);
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.World, _yak, Colour.White, new Vector2(0.0f, 10.0f), 220.0f, 196.0f, 0.5f);

        // Screen space is fixed to the camera's view, so the HUD shakes with it. Draw HUDs through a
        // second, still camera if they should stay put
        draw.DrawString(_drawStage, CoordinateSpace.World, "Press Space to shake", Colour.White, 30.0f,
                        new Vector2(0.0f, 220.0f), TextJustify.Centre, 0.5f, 0);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.SkyBlue);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class FadeTransition : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _city;
    private ITexture _yakPhoto;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _city = yak.Surfaces.LoadTexture("city", AssetSourceEnum.Embedded);
        _yakPhoto = yak.Surfaces.LoadTexture("yakphoto", AssetSourceEnum.Embedded);
        return true;
    }

    #region fade
    private enum Fade { None, Out, In }

    private const float FadeSeconds = 0.4f;
    private Fade _fade = Fade.None;
    private float _blackness;      // 0 = scene fully visible, 1 = fully black
    private int _scene;

    public override bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        if (_fade == Fade.None && yak.Input.WasKeyPressedThisFrame(KeyCode.Space))
        {
            _fade = Fade.Out;
        }

        var step = secondsSinceLastUpdate / FadeSeconds;
        switch (_fade)
        {
            case Fade.Out:
                _blackness = MathF.Min(1.0f, _blackness + step);
                if (_blackness == 1.0f)
                {
                    _scene = (_scene + 1) % 2; // swap scenes while the screen is black
                    _fade = Fade.In;
                }
                break;
            case Fade.In:
                _blackness = MathF.Max(0.0f, _blackness - step);
                if (_blackness == 0.0f)
                {
                    _fade = Fade.None;
                }
                break;
        }
        return base.Update(yak, secondsSinceLastUpdate);
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        var picture = _scene == 0 ? _city : _yakPhoto;
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, picture, Colour.White, Vector2.Zero, 960.0f, 540.0f, 0.9f);
        draw.DrawString(_drawStage, CoordinateSpace.Screen, $"Scene {_scene + 1}: press Space", Colour.White, 32.0f,
                        new Vector2(0.0f, 230.0f), TextJustify.Centre, 0.5f, 0);

        // A black rectangle over everything (a higher layer always draws on top), as opaque as the fade
        if (_blackness > 0.0f)
        {
            draw.Helpers.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, new Colour(0.0f, 0.0f, 0.0f, _blackness),
                                          Vector2.Zero, 960.0f, 540.0f, 0.0f, layer: 100);
        }
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.Black);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class Parallax : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _city;
    private ITexture _soil;
    private ITexture _grass;
    private ITexture _yak;
    private float _cameraX;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _city = yak.Surfaces.LoadTexture("city", AssetSourceEnum.Embedded);
        _soil = yak.Surfaces.LoadTexture("soil", AssetSourceEnum.Embedded);
        _grass = yak.Surfaces.LoadTexture("grass", AssetSourceEnum.Embedded);
        _yak = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);
        return true;
    }

    public override void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _cameraX += 160.0f * secondsSinceLastDraw; // the camera scrolls right forever
        yak.Cameras.SetCamera2DFocusAndZoom(_camera, new Vector2(_cameraX, 0.0f), 1.0f);
    }

    #region parallax
    // Draws a texture across the whole view, scrolled by a fraction of the camera's movement.
    // 0 = fixed to the screen (infinitely far away), 1 = moves with the world
    private void DrawBackdrop(IDrawingHelpers h, ITexture texture, Colour tint, float speed, float y, float height, float tileWidth, float depth)
    {
        var scroll = _cameraX * speed / tileWidth; // in texture widths; Wrap repeats the texture forever
        h.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, texture, tint, new Vector2(0.0f, y), 960.0f, height, depth,
                           texcoord_min_x: scroll, texcoord_max_x: scroll + (960.0f / tileWidth),
                           textureMode: TextureCoordinateMode.Wrap);
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        var h = draw.Helpers;

        // Far to near: each layer is nearer (lower depth) and scrolls faster than the one behind it.
        // Distant layers are tinted darker and drawn smaller, which also helps them read as far away
        DrawBackdrop(h, _city, Colour.White, 0.1f, 0.0f, 540.0f, 960.0f, 0.9f);
        DrawBackdrop(h, _soil, new Colour(0.45f, 0.5f, 0.45f, 1.0f), 0.4f, -105.0f, 50.0f, 64.0f, 0.85f);
        DrawBackdrop(h, _grass, new Colour(0.45f, 0.6f, 0.45f, 1.0f), 0.4f, -75.0f, 20.0f, 64.0f, 0.8f);
        DrawBackdrop(h, _soil, Colour.White, 1.0f, -215.0f, 110.0f, 128.0f, 0.7f);
        DrawBackdrop(h, _grass, Colour.White, 1.0f, -150.0f, 40.0f, 128.0f, 0.6f);

        // The yak lives in world space, keeping pace with the camera
        h.DrawTexturedQuad(_drawStage, CoordinateSpace.World, _yak, Colour.White, new Vector2(_cameraX - 200.0f, -80.0f), 120.0f, 107.0f, 0.5f);
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.Black);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class Particles : Example
{
    private ICamera2D _camera;
    private IDrawStage _background;
    private float _time;
    private float _nextBurst;
    private readonly Random _random = new Random(7);

    #region particles
    private struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Age;
        public float Life;
        public Colour Colour;
    }

    private readonly List<Particle> _particles = new List<Particle>();
    private IDrawStage _glow;

    public override bool CreateResources(IServices yak)
    {
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _background = yak.Stages.CreateDrawStage();

        // Additive blending: overlapping sparks add up and get brighter, like light does
        _glow = yak.Stages.CreateDrawStage(true, BlendState.AdditiveAlpha);
        return true;
    }

    private void Burst(Vector2 at, Colour colour)
    {
        for (var n = 0; n < 150; n++)
        {
            var angle = (float)_random.NextDouble() * MathF.Tau;
            var speed = 60.0f + (260.0f * (float)_random.NextDouble());
            _particles.Add(new Particle
            {
                Position = at,
                Velocity = speed * new Vector2(MathF.Cos(angle), MathF.Sin(angle)),
                Life = 0.8f + (0.8f * (float)_random.NextDouble()),
                Colour = colour,
            });
        }
    }

    public override bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        // A burst wherever the mouse is clicked, and one every so often on its own
        if (yak.Input.WasMousePressedThisFrame(MouseButton.Left))
        {
            var at = yak.Helpers.CoordinateTransforms.ScreenFromWindow(yak.Input.MousePosition, _camera).Position;
            Burst(at, Colour.Gold);
        }

        _time += secondsSinceLastUpdate;
        if (_time > _nextBurst)
        {
            var at = new Vector2(_random.Next(-380, 380), _random.Next(-120, 180));
            Burst(at, _random.Next(2) == 0 ? Colour.Orange : Colour.DeepSkyBlue);
            _nextBurst = _time + 0.6f;
        }

        // Move, slow down, fall, age. Remove from the end backwards so the indexes stay valid
        for (var n = _particles.Count - 1; n >= 0; n--)
        {
            var p = _particles[n];
            p.Velocity = (p.Velocity * (1.0f - (1.5f * secondsSinceLastUpdate))) + new Vector2(0.0f, -220.0f * secondsSinceLastUpdate);
            p.Position += p.Velocity * secondsSinceLastUpdate;
            p.Age += secondsSinceLastUpdate;
            if (p.Age >= p.Life)
            {
                _particles.RemoveAt(n);
            }
            else
            {
                _particles[n] = p;
            }
        }
        return base.Update(yak, secondsSinceLastUpdate);
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        foreach (var p in _particles)
        {
            // Fade out and shrink over the particle's life. With additive blending the colour is scaled
            // by its alpha before being added, so fading the alpha alone is enough
            var fade = 1.0f - (p.Age / p.Life);
            var colour = new Colour(p.Colour.R, p.Colour.G, p.Colour.B, fade);
            draw.Helpers.DrawColouredPoly(_glow, CoordinateSpace.Screen, colour, p.Position, 6, 3.0f + (5.0f * fade), 0.5f);
        }

        draw.DrawString(_background, CoordinateSpace.Screen, $"{_particles.Count} particles - click to add more", Colour.Gray, 22.0f,
                        new Vector2(0.0f, -230.0f), TextJustify.Centre, 0.5f, 0);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, new Colour(0.04f, 0.04f, 0.08f, 1.0f));
        q.ClearDepth(windowRenderTarget);
        q.Draw(_background, _camera, windowRenderTarget);
        q.ClearDepth(windowRenderTarget); // so the glow is never hidden behind the background's depth
        q.Draw(_glow, _camera, windowRenderTarget);
    }
    #endregion
}

public class HudOverlay : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _city;
    private float _time;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _city = yak.Surfaces.LoadTexture("city", AssetSourceEnum.Embedded);
        return true;
    }

    #region hud
    // Screen space has (0,0) in the centre, so the edges of a 960 x 540 camera are at x = +-480, y = +-270
    private const float Left = -480.0f;
    private const float Right = 480.0f;
    private const float Top = 270.0f;
    private const float Margin = 20.0f;

    // A bar that empties from right to left. Quads are positioned by their centre, so the
    // filled part's centre moves left as it shrinks
    private void DrawBar(IDrawingHelpers h, Vector2 topLeft, float width, float height, float amount, Colour colour)
    {
        var centre = topLeft + new Vector2(0.5f * width, -0.5f * height);
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, new Colour(0.0f, 0.0f, 0.0f, 0.6f), centre, width + 6.0f, height + 6.0f, 0.3f, 10);

        var filled = width * Math.Clamp(amount, 0.0f, 1.0f);
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, colour, topLeft + new Vector2(0.5f * filled, -0.5f * height), filled, height, 0.2f, 10);
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _city, Colour.White, Vector2.Zero, 960.0f, 540.0f, 0.9f);

        // Top left: a health bar that changes colour as it drops
        var health = 0.55f + (0.45f * MathF.Sin(0.8f * _time));
        var barColour = health > 0.3f ? Colour.LimeGreen : Colour.Red;
        DrawBar(draw.Helpers, new Vector2(Left + Margin, Top - Margin), 260.0f, 22.0f, health, barColour);

        // Top right: a score, right-justified so it grows leftwards from the corner
        var score = (int)(_time * 137.0f);
        draw.DrawString(_drawStage, CoordinateSpace.Screen, $"SCORE {score:000000}", Colour.White, 30.0f,
                        new Vector2(Right - Margin, Top - Margin + 4.0f), TextJustify.Right, 0.2f, 10);

        // Bottom left: frame rates, from the IFps given to Drawing()
        draw.DrawString(_drawStage, CoordinateSpace.Screen, $"{fps.DrawFPS:0} fps, {fps.UpdateFPS:0} updates/s", Colour.Yellow, 18.0f,
                        new Vector2(Left + Margin, -Top + Margin + 18.0f), TextJustify.Left, 0.2f, 10);
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.Black);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class WindowControls : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private float _spin;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        return true;
    }

    #region window-controls
    private bool _paused;

    public override void ProcessMessage(FrameworkMessage msg, IServices yak)
    {
        base.ProcessMessage(msg, yak); // keeps recreating resources after a device reset

        // Pause automatically when the player switches to another window
        if (msg == FrameworkMessage.WindowLostFocus)
        {
            _paused = true;
        }
    }

    public override bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        var input = yak.Input;

        // F11 toggles between a window and borderless full screen (which switches instantly,
        // without changing the display mode)
        if (input.WasKeyPressedThisFrame(KeyCode.F11))
        {
            var fullScreen = yak.Display.DisplayState == DisplayState.BorderlessFullScreen;
            yak.Display.SetDisplayState(fullScreen ? DisplayState.Normal : DisplayState.BorderlessFullScreen);
        }

        if (input.WasKeyPressedThisFrame(KeyCode.P))
        {
            _paused = !_paused;
        }

        if (!_paused)
        {
            _spin += secondsSinceLastUpdate; // the "game": skipped entirely while paused
        }
        return base.Update(yak, secondsSinceLastUpdate);
    }
    #endregion

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.Helpers.DrawColouredPoly(_drawStage, CoordinateSpace.Screen, Colour.Orange, Vector2.Zero, 5, 120.0f, 0.5f,
                                      rotation_clockwise_radians: _spin);
        draw.DrawString(_drawStage, CoordinateSpace.Screen, _paused ? "PAUSED (P to resume)" : "F11: full screen   P: pause",
                        Colour.White, 30.0f, new Vector2(0.0f, -190.0f), TextJustify.Centre, 0.4f, 0);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class TransparentLayer : Example
{
    private ICamera2D _camera;
    private IDrawStage _background;
    private ITexture _city;

    #region transparent-layer
    private IDrawStage _puffs;          // draws translucent shapes into the layer
    private IRenderTarget _layer;       // starts fully transparent each frame
    private IDrawStage _premultiplied;  // composites the layer correctly
    private IDrawStage _alpha;          // composites it the naive way, for comparison

    public override bool CreateResources(IServices yak)
    {
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _background = yak.Stages.CreateDrawStage();
        _city = yak.Surfaces.LoadTexture("city", AssetSourceEnum.Embedded);

        _puffs = yak.Stages.CreateDrawStage();
        _layer = yak.Surfaces.CreateRenderTarget(960, 540);
        _premultiplied = yak.Stages.CreateDrawStage(true, BlendState.Premultiplied);
        _alpha = yak.Stages.CreateDrawStage(true, BlendState.Alpha);
        return true;
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.Helpers.DrawTexturedQuad(_background, CoordinateSpace.Screen, _city, Colour.White, Vector2.Zero, 960.0f, 540.0f, 0.9f);

        // The same soft white clouds on both halves of the layer: overlapping, half-transparent circles
        foreach (var x in new[] { -240.0f, 240.0f })
        {
            for (var n = 0; n < 5; n++)
            {
                var puff = new Vector2(x - 120.0f + (n * 60.0f), 20.0f * MathF.Sin(n * 1.7f));
                draw.Helpers.DrawColouredPoly(_puffs, CoordinateSpace.Screen, new Colour(1.0f, 1.0f, 1.0f, 0.5f), puff, 48, 70.0f, 0.5f);
            }
        }

        // The layer is a texture, so it can be drawn like one. Left half: Premultiplied. Right half: Alpha
        draw.Helpers.DrawTexturedQuad(_premultiplied, CoordinateSpace.Screen, _layer, Colour.White, new Vector2(-240.0f, 0.0f), 480.0f, 540.0f, 0.5f,
                                      texcoord_max_x: 0.5f);
        draw.Helpers.DrawTexturedQuad(_alpha, CoordinateSpace.Screen, _layer, Colour.White, new Vector2(240.0f, 0.0f), 480.0f, 540.0f, 0.5f,
                                      texcoord_min_x: 0.5f);

        draw.DrawString(_background, CoordinateSpace.Screen, "Premultiplied", Colour.White, 30.0f, new Vector2(-240.0f, 220.0f), TextJustify.Centre, 0.5f, 0);
        draw.DrawString(_background, CoordinateSpace.Screen, "Alpha (too dark)", Colour.White, 30.0f, new Vector2(240.0f, 220.0f), TextJustify.Centre, 0.5f, 0);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        // 1. Draw the translucent shapes into the transparent layer
        q.ClearColour(_layer, Colour.Clear);
        q.Draw(_puffs, _camera, _layer);

        // 2. Draw the background, then the layer over it, onto the window
        q.ClearColour(windowRenderTarget, Colour.Black);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_background, _camera, windowRenderTarget);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_premultiplied, _camera, windowRenderTarget);
        q.Draw(_alpha, _camera, windowRenderTarget);
    }
    #endregion
}
