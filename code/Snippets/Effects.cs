using System.Numerics;
using Yak2D;

namespace Snippets;

// Guide: Post-processing effects (articles/effects.md)

// Shared helper: label each part of the window
public abstract class EffectExample : Example
{
    protected IDrawStage Labels;
    protected ICamera2D Screen;

    protected void CreateLabels(IServices yak)
    {
        Labels = yak.Stages.CreateDrawStage();
        Screen = yak.Cameras.CreateCamera2D(960, 540);
    }

    protected void Label(IDrawing draw, string text, Vector2 topCentre)
    {
        var width = draw.MeasureStringLength(text, 20.0f);
        draw.Helpers.DrawColouredQuad(Labels, CoordinateSpace.Screen, new Colour(0.0f, 0.0f, 0.0f, 0.7f),
                                      topCentre - new Vector2(0.0f, 12.0f), width + 16.0f, 30.0f, 0.6f);
        draw.DrawString(Labels, CoordinateSpace.Screen, text, Colour.White, 20.0f, topCentre, TextJustify.Centre, 0.5f, 0);
    }

    protected void RenderLabels(IRenderQueue q, IRenderTarget window)
    {
        q.RemoveViewport();
        q.ClearDepth(window);
        q.Draw(Labels, Screen, window);
    }
}

public class ColourEffectsExample : EffectExample
{
    private ITexture _yak;
    private IColourEffectsStage _grey;
    private IColourEffectsStage _tint;
    private IColourEffectsStage _negative;
    private IViewport[] _quarters;

    #region colour-resources
    public override bool CreateResources(IServices yak)
    {
        _yak = yak.Surfaces.LoadTexture("yakphoto", AssetSourceEnum.Embedded);

        // A stage has one configuration per frame, so three different looks need three stages
        _grey = yak.Stages.CreateColourEffectsStage();
        _tint = yak.Stages.CreateColourEffectsStage();
        _negative = yak.Stages.CreateColourEffectsStage();

        // Start from "no effect" (everything 0, full opacity) and switch on one effect at a time
        var none = new ColourEffectConfiguration { Opacity = 1.0f };

        var grey = none;
        grey.GrayScale = 1.0f;
        yak.Stages.SetColourEffectsConfig(_grey, grey);

        var tint = none;
        tint.Colourise = 1.0f;
        tint.ColourForSingleColourAndColourise = Colour.Orange;
        yak.Stages.SetColourEffectsConfig(_tint, tint);

        var negative = none;
        negative.Negative = 1.0f;
        yak.Stages.SetColourEffectsConfig(_negative, negative);

        CreateQuarters(yak);
        return true;
    }
    #endregion

    // Four viewports, one per quarter of the window, and the labels
    private void CreateQuarters(IServices yak)
    {
        _quarters = new[]
        {
            yak.Stages.CreateViewport(0, 0, 480, 270), yak.Stages.CreateViewport(480, 0, 480, 270),
            yak.Stages.CreateViewport(0, 270, 480, 270), yak.Stages.CreateViewport(480, 270, 480, 270),
        };
        CreateLabels(yak);
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        Label(draw, "Original (Copy)", new Vector2(-240.0f, 255.0f));
        Label(draw, "GrayScale", new Vector2(240.0f, 255.0f));
        Label(draw, "Colourise (orange)", new Vector2(-240.0f, -15.0f));
        Label(draw, "Negative", new Vector2(240.0f, -15.0f));
    }

    #region colour-rendering
    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        // Effects read a texture (any Texture or RenderTarget) and write to a render target. No draw stage needed
        q.SetViewport(_quarters[0]);
        q.Copy(_yak, windowRenderTarget);
        q.SetViewport(_quarters[1]);
        q.ColourEffects(_grey, _yak, windowRenderTarget);
        q.SetViewport(_quarters[2]);
        q.ColourEffects(_tint, _yak, windowRenderTarget);
        q.SetViewport(_quarters[3]);
        q.ColourEffects(_negative, _yak, windowRenderTarget);

        RenderLabels(q, windowRenderTarget);
    }
    #endregion
}

public class ColourTransition : Example
{
    private ITexture _city;
    private IColourEffectsStage _colour;
    private bool _grey;

    public override bool CreateResources(IServices yak)
    {
        _city = yak.Surfaces.LoadTexture("city", AssetSourceEnum.Embedded);
        _colour = yak.Stages.CreateColourEffectsStage();
        yak.Stages.SetColourEffectsConfig(_colour, new ColourEffectConfiguration { Opacity = 1.0f });
        return true;
    }

    #region colour-transition
    public override bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        if (yak.Input.WasKeyPressedThisFrame(KeyCode.Space))
        {
            _grey = !_grey;
            var target = new ColourEffectConfiguration { Opacity = 1.0f, GrayScale = _grey ? 1.0f : 0.0f };

            // The stage blends from its current settings to the new ones over 1.5 seconds
            yak.Stages.SetColourEffectsConfig(_colour, target, 1.5f);
        }
        return base.Update(yak, secondsSinceLastUpdate);
    }
    #endregion

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ColourEffects(_colour, _city, windowRenderTarget);
    }
}

public class BloomExample : EffectExample
{
    private IDrawStage _scene;
    private ICamera2D _camera;
    private IRenderTarget _target;
    private IBloomStage _bloom;
    private IViewport _left;
    private IViewport _right;

    #region bloom-resources
    public override bool CreateResources(IServices yak)
    {
        _scene = yak.Stages.CreateDrawStage();
        _camera = yak.Cameras.CreateCamera2D(480, 540);
        _target = yak.Surfaces.CreateRenderTarget(480, 540);

        // The bloom works on a smaller copy of the image: smaller is faster and spreads the glow further
        _bloom = yak.Stages.CreateBloomStage(120, 135);
        yak.Stages.SetBloomConfig(_bloom, new BloomEffectConfiguration
        {
            BrightnessThreshold = 0.6f,   // only pixels brighter than this (luminance, 0 to 1) glow
            AdditiveMixAmount = 1.5f,     // how strongly the glow is added back on top
            NumberOfBlurSamples = 8,      // up to 8
            ReSamplerType = ResizeSamplerType.Average4x4
        });
        #endregion

        _left = yak.Stages.CreateViewport(0, 0, 480, 540);
        _right = yak.Stages.CreateViewport(480, 0, 480, 540);
        CreateLabels(yak);
        return true;
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        // Neon shapes on a dark background, drawn once into a render target
        var h = draw.Helpers;
        h.DrawColouredQuad(_scene, CoordinateSpace.Screen, new Colour(0.05f, 0.05f, 0.12f, 1.0f), Vector2.Zero, 480.0f, 540.0f, 0.9f);
        h.Construct().Coloured(Colour.DeepPink).Poly(new Vector2(-90.0f, 120.0f), 48, 70.0f).Outline(10.0f).SubmitDraw(_scene, CoordinateSpace.Screen, 0.5f, 0);
        h.Construct().Coloured(Colour.Cyan).Quad(new Vector2(100.0f, 110.0f), 130.0f, 130.0f).Outline(10.0f).SubmitDraw(_scene, CoordinateSpace.Screen, 0.5f, 0);
        h.DrawColouredPoly(_scene, CoordinateSpace.Screen, Colour.Yellow, new Vector2(0.0f, -80.0f), 3, 80.0f, 0.5f);
        h.DrawColouredQuad(_scene, CoordinateSpace.Screen, new Colour(0.3f, 0.3f, 0.35f, 1.0f), new Vector2(0.0f, -200.0f), 360.0f, 30.0f, 0.5f);
        draw.DrawString(_scene, CoordinateSpace.Screen, "OPEN", Colour.Lime, 60.0f, new Vector2(0.0f, 20.0f), TextJustify.Centre, 0.5f, 0);

        Label(draw, "Copy", new Vector2(-240.0f, 255.0f));
        Label(draw, "Bloom", new Vector2(240.0f, 255.0f));
    }

    #region bloom-rendering
    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.Draw(_scene, _camera, _target);

        q.SetViewport(_left);
        q.Copy(_target, windowRenderTarget);

        q.SetViewport(_right);
        q.Bloom(_bloom, _target, windowRenderTarget);
        #endregion

        RenderLabels(q, windowRenderTarget);
    }
}

public class BlurExample : EffectExample
{
    private ITexture _city;
    private IBlurStage _blur;
    private IBlur1DStage _horizontal;
    private IBlur1DStage _diagonal;
    private IViewport[] _quarters;

    #region blur-resources
    public override bool CreateResources(IServices yak)
    {
        _city = yak.Surfaces.LoadTexture("city", AssetSourceEnum.Embedded);

        // As with bloom, the sample surface size trades quality for spread and speed
        _blur = yak.Stages.CreateBlurStage(240, 135);
        yak.Stages.SetBlurConfig(_blur, new BlurEffectConfiguration
        {
            MixAmount = 1.0f,   // 0 = original image, 1 = fully blurred
            NumberOfBlurSamples = 8,
            ReSamplerType = ResizeSamplerType.Average4x4
        });

        // Blur1D only blurs along one direction, e.g. for a sense of speed
        _horizontal = yak.Stages.CreateBlur1DStage(240, 135);
        yak.Stages.SetBlur1DConfig(_horizontal, new Blur1DEffectConfiguration
        {
            MixAmount = 1.0f,
            NumberOfBlurSamples = 8,
            BlurDirection = Vector2.UnitX,
            ReSamplerType = ResizeSamplerType.Average4x4
        });

        _diagonal = yak.Stages.CreateBlur1DStage(240, 135);
        yak.Stages.SetBlur1DConfig(_diagonal, new Blur1DEffectConfiguration
        {
            MixAmount = 1.0f,
            NumberOfBlurSamples = 8,
            BlurDirection = Vector2.Normalize(new Vector2(1.0f, 1.0f)),
            ReSamplerType = ResizeSamplerType.Average4x4
        });
        #endregion

        _quarters = new[]
        {
            yak.Stages.CreateViewport(0, 0, 480, 270), yak.Stages.CreateViewport(480, 0, 480, 270),
            yak.Stages.CreateViewport(0, 270, 480, 270), yak.Stages.CreateViewport(480, 270, 480, 270),
        };
        CreateLabels(yak);
        return true;
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        Label(draw, "Original", new Vector2(-240.0f, 255.0f));
        Label(draw, "Blur", new Vector2(240.0f, 255.0f));
        Label(draw, "Blur1D (horizontal)", new Vector2(-240.0f, -15.0f));
        Label(draw, "Blur1D (diagonal)", new Vector2(240.0f, -15.0f));
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.SetViewport(_quarters[0]);
        q.Copy(_city, windowRenderTarget);
        q.SetViewport(_quarters[1]);
        q.Blur(_blur, _city, windowRenderTarget);
        q.SetViewport(_quarters[2]);
        q.Blur1D(_horizontal, _city, windowRenderTarget);
        q.SetViewport(_quarters[3]);
        q.Blur1D(_diagonal, _city, windowRenderTarget);

        RenderLabels(q, windowRenderTarget);
    }
}

public class StyleEffectsExample : EffectExample
{
    private ITexture _yak;
    private IStyleEffectsStage _pixellate;
    private IStyleEffectsStage _edges;
    private IStyleEffectsStage _static;
    private IStyleEffectsStage _oldMovie;
    private IStyleEffectsStage _crt;
    private IViewport[] _cells;

    #region style-resources
    public override bool CreateResources(IServices yak)
    {
        _yak = yak.Surfaces.LoadTexture("yakphoto", AssetSourceEnum.Embedded);

        _pixellate = yak.Stages.CreateStyleEffectsStage();
        _edges = yak.Stages.CreateStyleEffectsStage();
        _static = yak.Stages.CreateStyleEffectsStage();
        _oldMovie = yak.Stages.CreateStyleEffectsStage();
        _crt = yak.Stages.CreateStyleEffectsStage();

        // Each effect has a PreSet(amount) helper that gives a sensible configuration for an amount from 0 to 1.
        // A new style stage has every effect switched off, so these each switch on just one
        yak.Stages.SetStyleEffectsPixellateConfig(_pixellate, PixellateConfiguration.PreSet(0.6f));
        yak.Stages.SetStyleEffectsEdgeDetectionConfig(_edges, EdgeDetectionConfiguration.PreSet(1.0f));
        yak.Stages.SetStyleEffectsStaticConfig(_static, StaticConfiguration.PreSet(0.5f));
        yak.Stages.SetStyleEffectsOldMovieConfig(_oldMovie, OldMovieConfiguration.PreSet(0.8f));
        yak.Stages.SetStyleEffectsCrtConfig(_crt, CrtEffectConfiguration.PreSet(1.0f, 320.0f / 270.0f));
        #endregion

        _cells = new IViewport[6];
        for (var n = 0; n < 6; n++)
        {
            _cells[n] = yak.Stages.CreateViewport((uint)(320 * (n % 3)), (uint)(270 * (n / 3)), 320, 270);
        }
        CreateLabels(yak);
        return true;
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        var names = new[] { "Original", "Pixellate", "Edge detection", "Static", "Old movie", "CRT" };
        for (var n = 0; n < 6; n++)
        {
            Label(draw, names[n], new Vector2(-320.0f + (320.0f * (n % 3)), 255.0f - (270.0f * (n / 3))));
        }
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.SetViewport(_cells[0]);
        q.Copy(_yak, windowRenderTarget);
        q.SetViewport(_cells[1]);
        q.StyleEffects(_pixellate, _yak, windowRenderTarget);
        q.SetViewport(_cells[2]);
        q.StyleEffects(_edges, _yak, windowRenderTarget);
        q.SetViewport(_cells[3]);
        q.StyleEffects(_static, _yak, windowRenderTarget);
        q.SetViewport(_cells[4]);
        q.StyleEffects(_oldMovie, _yak, windowRenderTarget);
        q.SetViewport(_cells[5]);
        q.StyleEffects(_crt, _yak, windowRenderTarget);

        RenderLabels(q, windowRenderTarget);
    }
}

public class MixExample : EffectExample
{
    private ITexture _city;
    private ITexture _yak;
    private ITexture _mask;
    private IMixStage _even;
    private IMixStage _masked;
    private IViewport[] _thirds;

    #region mix-resources
    public override bool CreateResources(IServices yak)
    {
        _city = yak.Surfaces.LoadTexture("city", AssetSourceEnum.Embedded);
        _yak = yak.Surfaces.LoadTexture("yakphoto", AssetSourceEnum.Embedded);

        // Half and half of the first two inputs
        _even = yak.Stages.CreateMixStage();
        yak.Stages.SetMixStageProperties(_even, new Vector4(0.5f, 0.5f, 0.0f, 0.0f));

        // Full amounts of each, but scaled per pixel by a mask texture
        _masked = yak.Stages.CreateMixStage();
        yak.Stages.SetMixStageProperties(_masked, new Vector4(1.0f, 1.0f, 0.0f, 0.0f));

        // The mask is generated in code: its R channel is input 0's share of each pixel, G is input 1's.
        // Here a circle of yak inside the city
        const int size = 128;
        var pixels = new Vector4[size * size];
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var distance = Vector2.Distance(new Vector2(x, y), new Vector2(size / 2)) / (size / 2);
                var yakAmount = Math.Clamp((0.8f - distance) * 5.0f, 0.0f, 1.0f);
                pixels[(y * size) + x] = new Vector4(1.0f - yakAmount, yakAmount, 0.0f, 0.0f);
            }
        }
        _mask = yak.Surfaces.CreateRgbaFromData(size, size, pixels);
        #endregion

        _thirds = new[]
        {
            yak.Stages.CreateViewport(0, 135, 320, 270), yak.Stages.CreateViewport(320, 135, 320, 270), yak.Stages.CreateViewport(640, 135, 320, 270),
        };
        CreateLabels(yak);
        return true;
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        Label(draw, "Mask texture", new Vector2(-320.0f, 120.0f));
        Label(draw, "Mix 50% / 50%", new Vector2(0.0f, 120.0f));
        Label(draw, "Mix using the mask", new Vector2(320.0f, 120.0f));
    }

    #region mix-rendering
    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.DarkSlateGray);

        q.SetViewport(_thirds[0]);
        q.Copy(_mask, windowRenderTarget);

        // Up to four inputs. Unused inputs, and the mask when it is not wanted, can be null
        q.SetViewport(_thirds[1]);
        q.Mix(_even, null, _city, _yak, null, null, windowRenderTarget);

        q.SetViewport(_thirds[2]);
        q.Mix(_masked, _mask, _city, _yak, null, null, windowRenderTarget);
        #endregion

        RenderLabels(q, windowRenderTarget);
    }
}

public class DistortionExample : Example
{
    private ITexture _city;
    private ITexture _ring;
    private IDistortionStage _distortion;
    private IDistortionCollection _ripples;
    private ICamera2D _camera;
    private float _time;

    #region distortion-resources
    public override bool CreateResources(IServices yak)
    {
        _city = yak.Surfaces.LoadTexture("city", AssetSourceEnum.Embedded);
        _camera = yak.Cameras.CreateCamera2D(960, 540);

        // The stage draws a height map at a lower resolution, then shifts the source image's pixels
        // according to the slope of that height map
        _distortion = yak.Stages.CreateDistortionStage(480, 270, true);
        yak.Stages.SetDistortionConfig(_distortion, new DistortionEffectConfiguration { DistortionScalar = 40.0f });

        // A ready made height map: rings, like a ripple on water
        _ring = yak.Helpers.DistortionHelper.TextureGenerator.ConcentricSinusoidalFloat32(256, 256, 8, true, true);
        _ripples = yak.Helpers.DistortionHelper.CreateNewCollection();
        return true;
    }
    #endregion

    #region distortion-update
    public override void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        _time += secondsSinceLastDraw;
        if (_time > 0.35f)
        {
            _time = 0.0f;
            var position = new Vector2(Random.Shared.Next(-400, 400), Random.Shared.Next(-220, 220));

            // Grow from nothing to 500 units across and fade out, over 2 seconds
            _ripples.Add(LifeCycle.Single, CoordinateSpace.Screen, 2.0f, _ring,
                         position, position, Vector2.Zero, new Vector2(500.0f),
                         1.0f, 0.0f, 0.0f, 0.0f);
        }
        _ripples.Update(secondsSinceLastDraw);
    }

    public override void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
    {
        // The collection submits one textured quad per ripple to the distortion stage
        _ripples.Draw(draw, _distortion);

        // Height map drawing can also be done directly, like a DrawStage. This one stays still
        draw.DrawDistortion(_distortion, CoordinateSpace.Screen, FillType.Textured,
                            new[]
                            {
                                new Vertex2D { Position = new Vector2(-150.0f, 150.0f), TexCoord0 = new Vector2(0.0f, 0.0f), Colour = Colour.White },
                                new Vertex2D { Position = new Vector2(150.0f, 150.0f), TexCoord0 = new Vector2(1.0f, 0.0f), Colour = Colour.White },
                                new Vertex2D { Position = new Vector2(150.0f, -150.0f), TexCoord0 = new Vector2(1.0f, 1.0f), Colour = Colour.White },
                                new Vertex2D { Position = new Vector2(-150.0f, -150.0f), TexCoord0 = new Vector2(0.0f, 1.0f), Colour = Colour.White },
                            },
                            new[] { 0, 1, 2, 0, 2, 3 },
                            Colour.White, _ring, null, TextureCoordinateMode.Mirror, TextureCoordinateMode.Mirror, 1.0f);
    }

    public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.Distortion(_distortion, _camera, _city, windowRenderTarget);
    }
    #endregion
}
