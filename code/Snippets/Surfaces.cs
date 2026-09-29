using System.Numerics;
using Yak2D;

namespace Snippets;

// Guide: Surfaces and render targets (articles/surfaces.md)

public class PixelArt : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _yak;
    private IRenderTarget _lowRes;
    private float _time;

    #region pixelart-resources
    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _yak = yak.Surfaces.LoadTexture("yak", AssetSourceEnum.Embedded);

        // The camera still works in 960 x 540 units...
        _camera = yak.Cameras.CreateCamera2D(960, 540);

        // ...but everything is drawn into a surface only 240 x 135 pixels in size, sampled with
        // Point filtering (no smoothing) when it is later stretched across the window
        _lowRes = yak.Surfaces.CreateRenderTarget(240, 135, true, SamplerType.Point);
        return true;
    }
    #endregion

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;
        var h = draw.Helpers;
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.SkyBlue, new Vector2(0.0f, 60.0f), 960.0f, 420.0f, 0.9f);
        h.DrawColouredQuad(_drawStage, CoordinateSpace.Screen, Colour.ForestGreen, new Vector2(0.0f, -210.0f), 960.0f, 120.0f, 0.9f);
        h.DrawColouredPoly(_drawStage, CoordinateSpace.Screen, Colour.Yellow, new Vector2(330.0f, 160.0f), 32, 60.0f, 0.8f);
        h.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _yak, Colour.White, new Vector2(-100.0f, -80.0f), 168.0f, 150.0f, 0.5f,
                           rotation_clockwise_radians: 0.1f * MathF.Sin(4.0f * _time));
        draw.DrawString(_drawStage, CoordinateSpace.Screen, "RETRO YAK", Colour.White, 60.0f, new Vector2(150.0f, 40.0f), TextJustify.Centre, 0.5f, 0);
    }

    #region pixelart-rendering
    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.Draw(_drawStage, _camera, _lowRes);  // draw into the small surface
        q.Copy(_lowRes, windowRenderTarget);    // stretch it over the window
    }
    #endregion
}

public class TextureFromData : Example
{
    private IDrawStage _drawStage;
    private ICamera2D _camera;
    private ITexture _checkerboard;
    private ITexture _yakCorner;

    #region texture-from-data
    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);

        // Build a texture from code: one Vector4 (R, G, B, A from 0 to 1) per pixel, row by row from the top-left
        const int size = 8;
        var pixels = new Vector4[size * size];
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                pixels[(y * size) + x] = (x + y) % 2 == 0 ? new Vector4(1.0f, 1.0f, 1.0f, 1.0f) : new Vector4(0.8f, 0.1f, 0.2f, 1.0f);
            }
        }
        _checkerboard = yak.Surfaces.CreateRgbaFromData(size, size, pixels, SamplerType.Point);

        // Or load an image's pixels, change them, and create a new texture from the result
        var data = yak.Surfaces.LoadTextureColourData("yakphoto", AssetSourceEnum.Embedded);
        var crop = new Vector4[240 * 160];
        for (var y = 0; y < 160; y++)
        {
            for (var x = 0; x < 240; x++)
            {
                var p = data.Pixels[((y + 40) * (int)data.Width) + x + 120];
                crop[(y * 240) + x] = new Vector4(p.Z, p.Y, p.X, p.W); // swap red and blue
            }
        }
        _yakCorner = yak.Surfaces.CreateRgbaFromData(240, 160, crop);
        return true;
    }
    #endregion

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _checkerboard, Colour.White, new Vector2(-220.0f, 0.0f), 320.0f, 320.0f, 0.5f);
        draw.Helpers.DrawTexturedQuad(_drawStage, CoordinateSpace.Screen, _yakCorner, Colour.White, new Vector2(220.0f, 0.0f), 360.0f, 240.0f, 0.5f);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_drawStage, _camera, windowRenderTarget);
    }
}

public class SurfaceReadback : Example
{
    private IDrawStage _drawStage;
    private IDrawStage _overlay;
    private ICamera2D _camera;
    private IRenderTarget _canvas;
    private ISurfaceCopyStage _readback;
    private Vector4[] _pixels;

    #region readback-resources
    public override bool CreateResources(IServices yak)
    {
        _drawStage = yak.Stages.CreateDrawStage();
        _overlay = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(960, 540);
        _canvas = yak.Surfaces.CreateRenderTarget(960, 540);

        // The callback runs once the GPU has finished rendering the frame and the pixels have been copied back
        _readback = yak.Stages.CreateSurfaceCopyDataStage(960, 540, data => _pixels = data.Pixels);
        return true;
    }
    #endregion

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        var colours = new[] { Colour.Crimson, Colour.Gold, Colour.MediumSeaGreen, Colour.DodgerBlue, Colour.Orchid };
        for (var n = 0; n < colours.Length; n++)
        {
            draw.Helpers.DrawColouredPoly(_drawStage, CoordinateSpace.Screen, colours[n], new Vector2(-360.0f + (180.0f * n), 0.0f), 48, 80.0f, 0.5f);
        }

        #region readback-use
        // Pixels are stored row by row from the top-left, the same as window coordinates
        if (_pixels != null)
        {
            var x = (int)Math.Clamp(input.MousePosition.X, 0, 959);
            var y = (int)Math.Clamp(input.MousePosition.Y, 0, 539);
            var pixel = _pixels[(y * 960) + x];

            var mouse = transforms.ScreenFromWindow(input.MousePosition, _camera).Position;
            draw.DrawString(_overlay, CoordinateSpace.Screen, $"R {pixel.X:0.00}  G {pixel.Y:0.00}  B {pixel.Z:0.00}",
                            Colour.White, 22.0f, mouse + new Vector2(0.0f, 60.0f), TextJustify.Centre, 0.5f, 0);
            draw.Helpers.DrawColouredQuad(_overlay, CoordinateSpace.Screen, new Colour(pixel.X, pixel.Y, pixel.Z, 1.0f),
                                          mouse + new Vector2(0.0f, 90.0f), 40.0f, 40.0f, 0.5f);
        }
        #endregion
    }

    #region readback-rendering
    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.Draw(_drawStage, _camera, _canvas);
        q.CopySurfaceData(_readback, _canvas); // ask for this surface's pixels
        q.Copy(_canvas, windowRenderTarget);
        q.ClearDepth(windowRenderTarget);
        q.Draw(_overlay, _camera, windowRenderTarget);
    }
    #endregion
}

// Guide: 3D meshes (articles/mesh.md)

public class MeshExample : Example
{
    private ITexture _yakPhoto;
    private ITexture _dice;
    private IMeshRenderStage _screen;
    private IMeshRenderStage _cube;
    private ICamera3D _camera;
    private IViewport _left;
    private IViewport _right;
    private float _angle;

    #region mesh-resources
    public override bool CreateResources(IServices yak)
    {
        _yakPhoto = yak.Surfaces.LoadTexture("yakphoto", AssetSourceEnum.Embedded);
        _dice = yak.Surfaces.LoadTexture("onetosix", AssetSourceEnum.Embedded);

        // A 3D camera: where it is, what it looks at, and which way is up (+Z points out of the screen)
        _camera = yak.Cameras.CreateCamera3D(new Vector3(0.0f, 0.0f, 300.0f), Vector3.Zero, Vector3.UnitY,
                                             60.0f, 480.0f / 540.0f, 1.0f, 1000.0f);

        // A curved, old-television shaped mesh, lit by one directional light
        _screen = yak.Stages.CreateMeshRenderStage();
        yak.Stages.SetMeshRenderMesh(_screen, yak.Helpers.CommonMeshBuilder.CreateCrtMesh(320.0f, 180.0f, 64, 0.3f, 0.3f));
        yak.Stages.SetMeshRenderLightingProperties(_screen, new MeshRenderLightingPropertiesConfiguration
        {
            NumberOfActiveLights = 1,
            Shininess = 20.0f,
            SpecularColour = Colour.White.ToVector3()
        });
        yak.Stages.SetMeshRenderLights(_screen, new[]
        {
            new MeshRenderLightConfiguration
            {
                LightType = LightType.Directional,
                Position = Vector3.Normalize(new Vector3(-1.0f, -1.0f, -1.0f)), // for directional lights: the direction
                Colour = Colour.White.ToVector3(),
                AmbientCoefficient = 0.2f
            }
        });

        // A cube, using the default lighting
        _cube = yak.Stages.CreateMeshRenderStage();
        #endregion

        _left = yak.Stages.CreateViewport(0, 0, 480, 540);
        _right = yak.Stages.CreateViewport(480, 0, 480, 540);
        return true;
    }

    #region mesh-update
    public override void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _angle += 60.0f * secondsSinceLastDraw; // degrees
        var cube = yak.Helpers.CommonMeshBuilder.CreateRectangularCuboidMesh(Vector3.Zero, 120.0f, 120.0f, 120.0f, _angle);
        yak.Stages.SetMeshRenderMesh(_cube, cube);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);
        q.ClearDepth(windowRenderTarget);

        // The source texture is wrapped over the mesh. It can be any surface, including a render
        // target, so a whole rendered 2D scene can be put onto a 3D object
        q.SetViewport(_left);
        q.MeshRender(_screen, _camera, _yakPhoto, windowRenderTarget);

        q.SetViewport(_right);
        q.MeshRender(_cube, _camera, _dice, windowRenderTarget);
    }
    #endregion
}
