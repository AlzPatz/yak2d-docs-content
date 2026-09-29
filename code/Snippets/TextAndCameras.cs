using System.Numerics;
using Yak2D;

namespace Snippets;

// Guide: Text (articles/text.md)

public class TextExample : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private IFont _snappy;

    #region text-resources
    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);

        // Loads every Fonts/snappy_*.fnt file (here snappy_38.fnt and snappy_64.fnt) and their page textures
        _snappy = yak.Fonts.LoadFont("snappy", AssetSourceEnum.Embedded);
        return true;
    }
    #endregion

    #region text-drawing
    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        // The built in font (Noto Sans) is used when no font is given
        draw.DrawString(_drawStage, CoordinateSpace.Screen, "The default font, 32 units high", Colour.White,
                        32.0f, new Vector2(-440.0f, 230.0f), TextJustify.Left, 0.5f, 0);

        // The position's y is the TOP of the text. x is the left edge, centre or right edge depending on justification
        var y = 150.0f;
        draw.DrawString(_drawStage, CoordinateSpace.Screen, "Left", Colour.LightGreen, 28.0f, new Vector2(0.0f, y), TextJustify.Left, 0.5f, 0);
        draw.DrawString(_drawStage, CoordinateSpace.Screen, "Centre", Colour.Gold, 28.0f, new Vector2(0.0f, y - 40.0f), TextJustify.Centre, 0.5f, 0);
        draw.DrawString(_drawStage, CoordinateSpace.Screen, "Right", Colour.LightPink, 28.0f, new Vector2(0.0f, y - 80.0f), TextJustify.Right, 0.5f, 0);
        draw.Helpers.DrawLine(_drawStage, CoordinateSpace.Screen, new Vector2(0.0f, y + 10.0f), new Vector2(0.0f, y - 120.0f), 2.0f, Colour.Gray, 0.6f);

        // A loaded bitmap font. yak2D picks the closest size it has and scales it
        draw.DrawString(_drawStage, CoordinateSpace.Screen, "A loaded font: snappy", Colour.Orange,
                        56.0f, new Vector2(0.0f, -30.0f), TextJustify.Centre, 0.5f, 0, _snappy);

        // Measuring text, here to draw a box that fits it. The letters hang down from the top position,
        // with most of the glyph within about 0.2 to 0.9 of the font size below it
        var message = "Measured to fit";
        var size = 30.0f;
        var top = new Vector2(0.0f, -150.0f);
        var width = draw.MeasureStringLength(message, size);
        draw.Helpers.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.SteelBlue,
                                      top - new Vector2(0.0f, 0.55f * size), width + 30.0f, 1.5f * size, 0.6f);
        draw.DrawString(_drawStage, CoordinateSpace.Screen, message, Colour.White, size, top, TextJustify.Centre, 0.5f, 0);
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

// Guide: Cameras (articles/cameras.md)

public class CameraExample : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _soil;
    private float _time;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _soil = yak.Surfaces.LoadTexture("soil", AssetSourceEnum.Embedded);
        return true;
    }

    #region camera-move
    public override void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;

        // Look at a point that moves in a circle, zoom in and out, and tilt a little
        var focus = new Vector2(200.0f * MathF.Cos(0.5f * _time), 120.0f * MathF.Sin(0.5f * _time));
        var zoom = 1.25f + (0.5f * MathF.Sin(0.7f * _time));
        var angle = 0.2f * MathF.Sin(0.3f * _time); // radians, clockwise

        yak.Cameras.SetCamera2DFocusZoomAndRotation(_camera, focus, zoom, angle);
    }
    #endregion

    #region camera-draw
    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        // World space: a floor, a grid of posts and a marker at the origin. These move with the camera
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.World, _soil, Colour.White, Vector2.Zero, 2000.0f, 1200.0f, 0.9f,
                                      texcoord_max_x: 2000.0f / 128.0f, texcoord_max_y: 1200.0f / 128.0f);
        for (var x = -800; x <= 800; x += 200)
        {
            for (var y = -400; y <= 400; y += 200)
            {
                draw.Helpers.DrawColouredPoly(_drawStage, CoordinateSpace.World, Colour.Wheat, new Vector2(x, y), 4, 20.0f, 0.5f);
            }
        }
        draw.DrawString(_drawStage, CoordinateSpace.World, "(0, 0)", Colour.White, 30.0f, new Vector2(0.0f, 60.0f), TextJustify.Centre, 0.5f, 0);
        draw.Helpers.DrawColouredPoly(_drawStage, CoordinateSpace.World, Colour.Red, Vector2.Zero, 32, 16.0f, 0.4f);

        // Screen space: a panel fixed to the top of the screen, unaffected by the camera's position, zoom and rotation
        draw.Helpers.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, new Colour(0.0f, 0.0f, 0.0f, 0.6f),
                                      new Vector2(0.0f, 240.0f), 960.0f, 60.0f, 0.3f, 1);
        draw.DrawString(_drawStage, CoordinateSpace.Screen, "Screen space: stays put", Colour.White, 26.0f,
                        new Vector2(0.0f, 255.0f), TextJustify.Centre, 0.2f, 1);
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.Black);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

// Guide: Viewports (articles/viewports.md)

public class SplitScreen : Example
{
    private IDrawStage _world;
    private IDrawStage _labels;
    private ICamera2D _left;
    private ICamera2D _right;
    private ICamera2D _screen;
    private IViewport _leftViewport;
    private IViewport _rightViewport;
    private ITexture _yak;
    private float _time;

    #region split-resources
    public override bool CreateResources(IServices yak)
    {
        _world = yak.Stages.CreateDrawStage();
        _labels = yak.Stages.CreateDrawStage();
        _yak = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);

        // Each half of the window is 480 x 540 pixels, so each camera gets a matching 480 x 540 virtual resolution
        _left = yak.Cameras.CreateCamera2D(480, 540);
        _right = yak.Cameras.CreateCamera2D(480, 540);
        _screen = yak.Cameras.CreateCamera2D(960, 540);

        // Viewports are in window pixels, from the top-left corner: (x, y, width, height)
        _leftViewport = yak.Stages.CreateViewport(0, 0, 480, 540);
        _rightViewport = yak.Stages.CreateViewport(480, 0, 480, 540);
        return true;
    }
    #endregion

    public override void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;
        yak.Cameras.SetCamera2DFocusAndZoom(_left, new Vector2(-250.0f + (40.0f * MathF.Sin(_time)), 0.0f), 1.0f);
        yak.Cameras.SetCamera2DFocusAndZoom(_right, new Vector2(250.0f, 0.0f), 2.0f);
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        // One world, drawn once...
        draw.Helpers.DrawColouredQuad(_world, CoordinateSpace.World, Colour.DarkOliveGreen, Vector2.Zero, 1400.0f, 800.0f, 0.9f);
        draw.Helpers.DrawTexturedQuad(_world, CoordinateSpace.World, _yak, Colour.White, new Vector2(-250.0f, 0.0f), 168.0f, 150.0f, 0.5f);
        draw.Helpers.DrawTexturedQuad(_world, CoordinateSpace.World, _yak, Colour.LightSalmon, new Vector2(250.0f, 0.0f), 168.0f, 150.0f, 0.5f);

        draw.DrawString(_labels, CoordinateSpace.Screen, "Player 1", Colour.White, 28.0f, new Vector2(-240.0f, 250.0f), TextJustify.Centre, 0.5f, 0);
        draw.DrawString(_labels, CoordinateSpace.Screen, "Player 2 (zoomed x2)", Colour.White, 28.0f, new Vector2(240.0f, 250.0f), TextJustify.Centre, 0.5f, 0);
        draw.Helpers.DrawLine(_labels, CoordinateSpace.Screen, new Vector2(0.0f, 270.0f), new Vector2(0.0f, -270.0f), 6.0f, Colour.Black, 0.5f);
    }

    #region split-rendering
    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.Black);
        q.ClearDepth(windowRenderTarget);

        // ...rendered twice, into different parts of the window, through different cameras
        q.SetViewport(_leftViewport);
        q.Draw(_world, _left, windowRenderTarget);

        q.SetViewport(_rightViewport);
        q.Draw(_world, _right, windowRenderTarget);

        // Back to the whole window for the labels
        q.RemoveViewport();
        q.ClearDepth(windowRenderTarget);
        q.Draw(_labels, _screen, windowRenderTarget);
    }
    #endregion
}

// Guide: Coordinate systems (articles/coordinatesystems.md)

public class MousePicking : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private readonly List<Vector2> _placed = new List<Vector2>();

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        yak.Cameras.SetCamera2DFocusZoomAndRotation(_camera, new Vector2(300.0f, 100.0f), 1.5f, 0.3f);
        return true;
    }

    #region picking
    public override bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        var input = yak.Input;
        if (input.WasMousePressedThisFrame(MouseButton.Left))
        {
            // Mouse positions are window pixels from the top-left. Convert to the camera's world space
            var world = yak.Helpers.CoordinateTransforms.WorldFromWindow(input.MousePosition, _camera);
            if (world.Contained)
            {
                _placed.Add(world.Position);
            }
        }
        return base.Update(yak, secondsSinceLastUpdate);
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        // A world space grid, seen through a zoomed and rotated camera
        for (var n = -10; n <= 10; n++)
        {
            draw.Helpers.DrawLine(_drawStage, CoordinateSpace.World, new Vector2(n * 100.0f, -1000.0f), new Vector2(n * 100.0f, 1000.0f), 2.0f, Colour.DimGray, 0.9f);
            draw.Helpers.DrawLine(_drawStage, CoordinateSpace.World, new Vector2(-1000.0f, n * 100.0f), new Vector2(1000.0f, n * 100.0f), 2.0f, Colour.DimGray, 0.9f);
        }

        foreach (var p in _placed)
        {
            draw.Helpers.DrawColouredPoly(_drawStage, CoordinateSpace.World, Colour.Gold, p, 5, 14.0f, 0.5f);
        }

        // The cursor, in world space and screen space
        var world = transforms.WorldFromWindow(input.MousePosition, _camera);
        var screen = transforms.ScreenFromWindow(input.MousePosition, _camera);
        if (world.Contained)
        {
            draw.Helpers.DrawColouredPoly(_drawStage, CoordinateSpace.World, Colour.Red, world.Position, 32, 10.0f, 0.4f);
            draw.DrawString(_drawStage, CoordinateSpace.Screen,
                            $"window {input.MousePosition:0}  screen {screen.Position:0}  world {world.Position:0}",
                            Colour.White, 20.0f, new Vector2(0.0f, -230.0f), TextJustify.Centre, 0.3f, 1);
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
