using System.Numerics;
using Yak2D;

namespace YakRun;

#region star
public class Star
{
    public Vector2 Position;
    public bool Collected;
}
#endregion

public class Level
{
    #region data
    public const float GrassHeight = 28.0f;

    // Falling below this height loses the yak
    public const float FallLimit = -800.0f;

    public Vector2 Start { get; } = new Vector2(-250.0f, -120.0f);
    public Vector2 Flag { get; } = new Vector2(5150.0f, -150.0f);

    public List<Box> Platforms { get; } = new List<Box>
    {
        new Box(-1400.0f, -700.0f, 160.0f, 1300.0f),   // a tall wall at the far left
        new Box(-1300.0f, -700.0f, 2000.0f, 550.0f),
        new Box(820.0f, -700.0f, 500.0f, 500.0f),
        new Box(1420.0f, -120.0f, 240.0f, 50.0f),
        new Box(1760.0f, -700.0f, 420.0f, 580.0f),
        new Box(2280.0f, -40.0f, 200.0f, 40.0f),
        new Box(2560.0f, 80.0f, 200.0f, 40.0f),
        new Box(2840.0f, -60.0f, 240.0f, 40.0f),
        new Box(3180.0f, -700.0f, 700.0f, 530.0f),
        new Box(3980.0f, -700.0f, 300.0f, 620.0f),
        new Box(4380.0f, -700.0f, 1050.0f, 550.0f),
    };

    public List<Star> Stars { get; } = new[]
    {
        new Vector2(300.0f, -40.0f), new Vector2(760.0f, 0.0f), new Vector2(1070.0f, -90.0f),
        new Vector2(1540.0f, 40.0f), new Vector2(1970.0f, 0.0f), new Vector2(2380.0f, 90.0f),
        new Vector2(2660.0f, 210.0f), new Vector2(2960.0f, 70.0f), new Vector2(3530.0f, -60.0f),
        new Vector2(3930.0f, 60.0f), new Vector2(4130.0f, 30.0f), new Vector2(4700.0f, -40.0f),
    }.Select(p => new Star { Position = p }).ToList();
    #endregion

    public void Reset()
    {
        Stars.ForEach(s => s.Collected = false);
    }

    #region draw
    public void Draw(IDrawing draw, IDrawStage stage, ITexture grass, ITexture soil, float time)
    {
        foreach (var platform in Platforms)
        {
            DrawPlatform(draw, stage, platform, grass, soil);
        }

        foreach (var star in Stars.Where(s => !s.Collected))
        {
            // Each star bobs gently and spins
            var position = star.Position + new Vector2(0.0f, 6.0f * MathF.Sin((3.0f * time) + star.Position.X));
            DrawStar(draw, stage, position, 2.0f * time);
        }

        // The finishing flag: a pole (a line) and a triangle (a three sided polygon)
        var poleTop = Flag + new Vector2(0.0f, 220.0f);
        draw.Helpers.DrawLine(stage, CoordinateSpace.World, Flag, poleTop, 8.0f, Colour.WhiteSmoke, 0.6f, 1, rounded: true);
        draw.Helpers.DrawColouredPoly(stage, CoordinateSpace.World, Colour.Crimson, poleTop + new Vector2(40.0f, -30.0f), 3, 44.0f,
                                      depth: 0.6f, layer: 1, xScaling: 1.0f, yScaling: 0.7f,
                                      rotation_clockwise_radians: MathF.PI / 2.0f);
    }

    // A five pointed star built from our own vertices: ten points round the outside (alternating
    // long and short) plus one in the middle, joined up as ten triangles
    private static void DrawStar(IDrawing draw, IDrawStage stage, Vector2 centre, float angle)
    {
        var vertices = new Vector2[11];
        var indices = new int[30];
        vertices[0] = centre;
        for (var n = 0; n < 10; n++)
        {
            var radius = n % 2 == 0 ? 26.0f : 11.0f;
            var a = angle + (n * MathF.PI / 5.0f);
            vertices[n + 1] = centre + (radius * new Vector2(MathF.Sin(a), MathF.Cos(a)));

            indices[(3 * n) + 0] = 0;
            indices[(3 * n) + 1] = n + 1;
            indices[(3 * n) + 2] = ((n + 1) % 10) + 1;
        }

        draw.Helpers.Construct()
                    .Coloured(new Colour(1.0f, 0.92f, 0.4f, 1.0f))
                    .Poly(vertices, indices)
                    .Filled()
                    .SubmitDraw(stage, CoordinateSpace.World, 0.4f, 1);
    }

    private static void DrawPlatform(IDrawing draw, IDrawStage stage, Box box, ITexture grass, ITexture soil)
    {
        // Soil: the texture repeats every 128 units in both directions. Texture coordinates above 1.0
        // wrap round (TextureCoordinateMode.Wrap), so the maximum coordinate is simply size / 128
        draw.Helpers.DrawTexturedQuad(stage, CoordinateSpace.World, soil, Colour.White,
                                      box.Centre, box.Width, box.Height,
                                      depth: 0.8f, layer: 1,
                                      texcoord_max_x: box.Width / 128.0f,
                                      texcoord_max_y: box.Height / 128.0f,
                                      textureMode: TextureCoordinateMode.Wrap);

        // Grass: a strip along the top edge, repeating horizontally only, drawn in front of the soil (lower depth).
        // Mirror (rather than Wrap) stops the texture's bottom row bleeding into the transparent top edge
        var grassCentre = new Vector2(box.Centre.X, box.Top - (0.5f * GrassHeight) + 8.0f);
        draw.Helpers.DrawTexturedQuad(stage, CoordinateSpace.World, grass, Colour.White,
                                      grassCentre, box.Width, GrassHeight + 16.0f,
                                      depth: 0.7f, layer: 1,
                                      texcoord_max_x: box.Width / 128.0f,
                                      textureMode: TextureCoordinateMode.Mirror);
    }
    #endregion
}
