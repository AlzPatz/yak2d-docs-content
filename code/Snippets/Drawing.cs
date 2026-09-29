using System.Numerics;
using Yak2D;

namespace Snippets;

// Guide: Drawing (articles/drawing.md)

public class DrawingBasics : Example
{
    #region basics-resources
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _yak;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _yak = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);
        return true;
    }
    #endregion

    #region basics-drawing
    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        var helpers = draw.Helpers;

        // Rectangles ("quads"): coloured, and textured. Positions are the centre of the shape
        helpers.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.CornflowerBlue,
                                 new Vector2(-330.0f, 130.0f), 200.0f, 140.0f, 0.5f);

        helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _yak, Colour.White,
                                 new Vector2(-80.0f, 130.0f), 190.0f, 170.0f, 0.5f);

        // Tinting: the texture colour is multiplied by the colour given
        helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _yak, Colour.LightGreen,
                                 new Vector2(170.0f, 130.0f), 190.0f, 170.0f, 0.5f,
                                 rotation_clockwise_radians: 0.3f);

        // Regular polygons: a triangle, a hexagon, and enough sides to look like a circle
        helpers.DrawColouredPoly(_drawStage, CoordinateSpace.Screen, Colour.Orange, new Vector2(390.0f, 140.0f), 3, 70.0f, 0.5f);
        helpers.DrawColouredPoly(_drawStage, CoordinateSpace.Screen, Colour.MediumPurple, new Vector2(-360.0f, -120.0f), 6, 75.0f, 0.5f);
        helpers.DrawColouredPoly(_drawStage, CoordinateSpace.Screen, Colour.Crimson, new Vector2(-170.0f, -120.0f), 64, 70.0f, 0.5f,
                                 xScaling: 1.0f, yScaling: 0.6f); // squashed into an ellipse

        // Lines and arrows
        helpers.DrawLine(_drawStage, CoordinateSpace.Screen, new Vector2(-40.0f, -60.0f), new Vector2(120.0f, -180.0f),
                         16.0f, Colour.Teal, 0.5f, rounded: true);
        helpers.DrawArrow(_drawStage, CoordinateSpace.Screen, new Vector2(200.0f, -180.0f), new Vector2(420.0f, -60.0f),
                          18.0f, 60.0f, 60.0f, Colour.Gold, 0.5f);
    }
    #endregion

    #region basics-rendering
    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
    #endregion
}

public class LayersAndDepth : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        return true;
    }

    #region layers
    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        var h = draw.Helpers;
        var translucent = new Colour(1.0f, 1.0f, 1.0f, 0.6f);

        // Left: all on layer 0, so depth decides. Lower depth is nearer the viewer
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.Red, new Vector2(-300.0f, 40.0f), 200.0f, 200.0f, depth: 0.9f, layer: 0);
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.Green, new Vector2(-240.0f, -20.0f), 200.0f, 200.0f, depth: 0.5f, layer: 0);
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.Blue, new Vector2(-180.0f, -80.0f), 200.0f, 200.0f, depth: 0.1f, layer: 0);

        // Right: the same depths, but the red square is on a higher layer, so it is drawn on top of both
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.Red, new Vector2(180.0f, 40.0f), 200.0f, 200.0f, depth: 0.9f, layer: 1);
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.Green, new Vector2(240.0f, -20.0f), 200.0f, 200.0f, depth: 0.5f, layer: 0);
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.Blue, new Vector2(300.0f, -80.0f), 200.0f, 200.0f, depth: 0.1f, layer: 0);

        // Translucent shapes blend with whatever is behind them, whatever order they were submitted in,
        // because each stage sorts its requests back to front before drawing
        h.DrawColouredPoly(_drawStage, CoordinateSpace.Screen, translucent, new Vector2(-240.0f, 60.0f), 48, 70.0f, depth: 0.0f, layer: 0);
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class DrawRequests : Example
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

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        #region coloured-request
        // One triangle, with a different colour at each corner. The GPU blends between them
        var triangle = new DrawRequest
        {
            CoordinateSpace = CoordinateSpace.Screen,
            FillType = FillType.Coloured,
            Colour = Colour.White, // multiplied with every vertex colour
            Vertices = new[]
            {
                new Vertex2D { Position = new Vector2(-300.0f, 200.0f), Colour = Colour.Red },
                new Vertex2D { Position = new Vector2(-150.0f, -60.0f), Colour = Colour.Lime },
                new Vertex2D { Position = new Vector2(-450.0f, -60.0f), Colour = Colour.Blue },
            },
            Indices = new[] { 0, 1, 2 }, // groups of three indices into Vertices, one group per triangle
            Depth = 0.5f,
            Layer = 0,
        };
        draw.Draw(_drawStage, triangle);
        #endregion

        #region textured-request
        // A textured rectangle made of two triangles that share two of the four vertices.
        // Texture coordinates run from (0,0) at the top-left of the texture to (1,1) at the bottom-right
        var picture = new DrawRequest
        {
            CoordinateSpace = CoordinateSpace.Screen,
            FillType = FillType.Textured,
            Colour = Colour.White,
            Texture0 = _city,
            TextureWrap0 = TextureCoordinateMode.Wrap,
            Vertices = new[]
            {
                new Vertex2D { Position = new Vector2(-100.0f, 200.0f), TexCoord0 = new Vector2(0.0f, 0.0f), Colour = Colour.White },
                new Vertex2D { Position = new Vector2(180.0f, 200.0f), TexCoord0 = new Vector2(1.0f, 0.0f), Colour = Colour.White },
                new Vertex2D { Position = new Vector2(180.0f, 40.0f), TexCoord0 = new Vector2(1.0f, 1.0f), Colour = Colour.White },
                new Vertex2D { Position = new Vector2(-100.0f, 40.0f), TexCoord0 = new Vector2(0.0f, 1.0f), Colour = Colour.White },
            },
            Indices = new[] { 0, 1, 2, 0, 2, 3 },
            Depth = 0.5f,
            Layer = 0,
        };
        draw.Draw(_drawStage, picture);
        #endregion

        #region dual-textured-request
        // Dual texturing blends two textures. Each vertex's TexWeighting is how much of Texture0 to use
        // (the rest is Texture1), so here the yak on the left fades into the city on the right
        var blend = new DrawRequest
        {
            CoordinateSpace = CoordinateSpace.Screen,
            FillType = FillType.DualTextured,
            Colour = Colour.White,
            Texture0 = _yakPhoto,
            Texture1 = _city,
            TextureWrap0 = TextureCoordinateMode.Wrap,
            TextureWrap1 = TextureCoordinateMode.Wrap,
            Vertices = new[]
            {
                new Vertex2D { Position = new Vector2(-100.0f, -20.0f), TexCoord0 = new Vector2(0.0f, 0.0f), TexCoord1 = new Vector2(0.0f, 0.0f), TexWeighting = 1.0f, Colour = Colour.White },
                new Vertex2D { Position = new Vector2(420.0f, -20.0f), TexCoord0 = new Vector2(1.0f, 0.0f), TexCoord1 = new Vector2(1.0f, 0.0f), TexWeighting = 0.0f, Colour = Colour.White },
                new Vertex2D { Position = new Vector2(420.0f, -220.0f), TexCoord0 = new Vector2(1.0f, 1.0f), TexCoord1 = new Vector2(1.0f, 1.0f), TexWeighting = 0.0f, Colour = Colour.White },
                new Vertex2D { Position = new Vector2(-100.0f, -220.0f), TexCoord0 = new Vector2(0.0f, 1.0f), TexCoord1 = new Vector2(0.0f, 1.0f), TexWeighting = 1.0f, Colour = Colour.White },
            },
            Indices = new[] { 0, 1, 2, 0, 2, 3 },
            Depth = 0.5f,
            Layer = 0,
        };
        draw.Draw(_drawStage, blend);
        #endregion
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class FluentShapes : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _city;
    private ITexture _yakPhoto;
    private ITexture _soil;
    private float _time;

    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _city = yak.Surfaces.LoadTexture("city", AssetSourceEnum.Embedded);
        _yakPhoto = yak.Surfaces.LoadTexture("yakphoto", AssetSourceEnum.Embedded);
        _soil = yak.Surfaces.LoadTexture("soil", AssetSourceEnum.Embedded);
        return true;
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;
        var h = draw.Helpers;

        #region fluent-basic
        // Construct() -> fill type -> shape -> Filled() or Outline() -> draw
        h.Construct()
         .Coloured(Colour.Orange)
         .Poly(new Vector2(-360.0f, 150.0f), 5, 80.0f)
         .Outline(12.0f)
         .SubmitDraw(_drawStage, CoordinateSpace.Screen, 0.5f, 0);

        h.Construct()
         .Coloured(Colour.HotPink)
         .Line(new Vector2(-200.0f, 90.0f), new Vector2(-40.0f, 210.0f), 14.0f)
         .Arrow(40.0f, 50.0f)
         .Filled()
         .SubmitDraw(_drawStage, CoordinateSpace.Screen, 0.5f, 0);
        #endregion

        #region fluent-textured
        // A texture brush says how a texture covers a shape: stretched to fit, or tiled at a set scale
        var stretched = new TextureBrush(_city, TextureCoordinateMode.Wrap, TextureScaling.Stretch, Vector2.One);
        var tiled = new TextureBrush(_soil, TextureCoordinateMode.Wrap, TextureScaling.Tiled, new Vector2(1.0f, 1.0f));

        h.Construct().Textured(stretched, Colour.White).Poly(new Vector2(120.0f, 150.0f), 8, 90.0f).Filled()
         .SubmitDraw(_drawStage, CoordinateSpace.Screen, 0.5f, 0);

        h.Construct().Textured(tiled, Colour.White).Quad(new Vector2(350.0f, 150.0f), 200.0f, 160.0f).Filled()
         .SubmitDraw(_drawStage, CoordinateSpace.Screen, 0.5f, 0);

        // Two brushes blended across the shape: here from the middle (yak) outwards (city)
        var yakBrush = new TextureBrush(_yakPhoto, TextureCoordinateMode.Wrap, TextureScaling.Stretch, Vector2.One);
        h.Construct().DualTextured(yakBrush, stretched, TextureMixDirection.Radial, Colour.White)
         .Poly(new Vector2(-360.0f, -130.0f), 48, 100.0f).Filled()
         .SubmitDraw(_drawStage, CoordinateSpace.Screen, 0.5f, 0);
        #endregion

        #region fluent-transform
        // Each step returns a new, modified copy, so a shape can be built once and stamped out many times
        var square = h.Construct().Coloured(Colour.Aqua).Quad(new Vector2(-120.0f, -130.0f), 60.0f, 60.0f).Filled();
        for (var n = 0; n < 6; n++)
        {
            square = square.ShiftPosition(new Vector2(80.0f, 0.0f))
                           .Rotate(0.25f + (0.1f * _time))
                           .ChangeColour(Colour.Aqua * (1.0f - (n * 0.12f)));
            draw.Draw(_drawStage, square.GenerateDrawRequest(CoordinateSpace.Screen, 0.5f, 0));
        }
        #endregion
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class TextureModes : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _raccoon;
    private ITexture _raccoonPoint;

    #region texture-modes
    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);

        // The same image, loaded twice with different sampler filters
        _raccoon = yak.Surfaces.LoadTexture("raccoon", AssetSourceEnum.Embedded);
        _raccoonPoint = yak.Surfaces.LoadTexture("raccoon", AssetSourceEnum.Embedded, ImageFormat.PNG, SamplerType.Point, false);
        return true;
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        var h = draw.Helpers;

        // Texture coordinates from -1 to 2 show the texture three times across and down.
        // Wrap repeats it, Mirror flips every other copy
        h.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _raccoon, Colour.White, new Vector2(-300.0f, 60.0f), 300.0f, 400.0f, 0.5f,
                           texcoord_min_x: -1.0f, texcoord_min_y: -1.0f, texcoord_max_x: 2.0f, texcoord_max_y: 2.0f,
                           textureMode: TextureCoordinateMode.Wrap);

        h.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _raccoon, Colour.White, new Vector2(40.0f, 60.0f), 300.0f, 400.0f, 0.5f,
                           texcoord_min_x: -1.0f, texcoord_min_y: -1.0f, texcoord_max_x: 2.0f, texcoord_max_y: 2.0f,
                           textureMode: TextureCoordinateMode.Mirror);

        // Swapping min and max x flips the image horizontally. Drawing a small texture very large shows
        // the difference between point sampling (hard pixels) and the default (smooth) filtering
        h.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _raccoonPoint, Colour.White, new Vector2(330.0f, 60.0f), 220.0f, 400.0f, 0.5f,
                           texcoord_min_x: 0.62f, texcoord_min_y: 0.35f, texcoord_max_x: 0.42f, texcoord_max_y: 0.55f);
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class PersistentQueue : Example
{
    private IDrawStage _staticStage;
    private IDrawStage _dynamicStage;
    private ICamera2D _camera;
    private float _time;

    #region persistent
    public override bool CreateResources(IServices yak)
    {
        // This stage is NOT cleared automatically each frame: whatever is drawn to it stays until
        // ClearDynamicDrawRequestQueue() is called. Unchanged queues are not re-sorted or re-uploaded
        _staticStage = yak.Stages.CreateDrawStage(clearDynamicRequestQueueEachFrame: false);

        // The default: a stage that starts empty every frame
        _dynamicStage = yak.Stages.CreateDrawStage();

        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _hasDrawnStaticScene = false;
        return true;
    }

    private bool _hasDrawnStaticScene;

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms,
                                 float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;

        if (!_hasDrawnStaticScene)
        {
            // A few thousand shapes, submitted once
            var random = new Random(1);
            for (var n = 0; n < 3000; n++)
            {
                var position = new Vector2(random.Next(-480, 480), random.Next(-270, 270));
                var colour = new Colour(0.2f, 0.3f + (0.5f * (float)random.NextDouble()), 0.4f, 1.0f);
                draw.Helpers.DrawColouredPoly(_staticStage, CoordinateSpace.Screen, colour, position, 6, 6.0f, 0.8f);
            }
            _hasDrawnStaticScene = true;
        }

        // Moving things are drawn every frame to the normal stage
        var x = 380.0f * MathF.Sin(_time);
        draw.Helpers.DrawColouredPoly(_dynamicStage, CoordinateSpace.Screen, Colour.Orange, new Vector2(x, 0.0f), 48, 50.0f, 0.5f);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_staticStage, _camera, windowRenderTarget);
        q.Draw(_dynamicStage, _camera, windowRenderTarget);
    }
    #endregion
}
