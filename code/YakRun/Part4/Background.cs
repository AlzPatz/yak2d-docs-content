using System.Numerics;
using Yak2D;

namespace YakRun;

// Scenery behind the level. It is drawn by its own draw stage and viewed through its own camera,
// which moves more slowly than the main camera, so the background appears further away (parallax)
public class Background
{
    #region sky
    public const float ParallaxFactor = 0.3f;

    // A screen-sized rectangle with a different colour at each corner. The GPU blends between
    // vertex colours, which gives a gradient for free. It is built once and re-submitted every frame
    private readonly DrawRequest _sky = new DrawRequest
    {
        CoordinateSpace = CoordinateSpace.Screen,
        FillType = FillType.Coloured,
        Colour = Colour.White,
        Vertices = new[]
        {
            new Vertex2D { Position = new Vector2(-480.0f, 270.0f), Colour = new Colour(0.20f, 0.40f, 0.85f, 1.0f) },
            new Vertex2D { Position = new Vector2(480.0f, 270.0f), Colour = new Colour(0.20f, 0.40f, 0.85f, 1.0f) },
            new Vertex2D { Position = new Vector2(480.0f, -270.0f), Colour = new Colour(0.75f, 0.88f, 1.00f, 1.0f) },
            new Vertex2D { Position = new Vector2(-480.0f, -270.0f), Colour = new Colour(0.75f, 0.88f, 1.00f, 1.0f) },
        },
        Indices = new[] { 0, 1, 2, 0, 2, 3 },
        Depth = 0.95f,
        Layer = 0,
    };
    #endregion

    #region draw
    public void Draw(IDrawing draw, IDrawStage stage)
    {
        draw.Draw(stage, _sky);

        // The sun is in Screen space, so it stays put however the camera moves
        draw.Helpers.DrawColouredPoly(stage, CoordinateSpace.Screen, new Colour(1.0f, 0.95f, 0.6f, 1.0f),
                                      new Vector2(330.0f, 160.0f), 48, 55.0f, depth: 0.9f);

        // Mountains and hills are World space triangles (three sided regular polygons, stretched)
        for (var n = 0; n < 18; n++)
        {
            var x = -900.0f + (n * 230.0f);
            var height = 230.0f + (70.0f * MathF.Sin(n * 1.7f));
            DrawMountain(draw, stage, new Vector2(x, -200.0f), height + 50.0f, 1.5f, new Colour(0.55f, 0.60f, 0.75f, 1.0f), 0.8f);
            DrawMountain(draw, stage, new Vector2(x + 115.0f, -360.0f), (0.6f * height) + 140.0f, 2.4f, new Colour(0.35f, 0.55f, 0.40f, 1.0f), 0.7f);
        }

        // Clouds: a few overlapping circles (a many sided polygon approximates a circle)
        for (var n = 0; n < 8; n++)
        {
            var centre = new Vector2(-600.0f + (n * 420.0f), 150.0f + (40.0f * MathF.Sin(n * 2.3f)));
            var colour = new Colour(0.80f, 0.82f, 0.88f, 1.0f);
            draw.Helpers.DrawColouredPoly(stage, CoordinateSpace.World, colour, centre, 32, 38.0f, 0.6f);
            draw.Helpers.DrawColouredPoly(stage, CoordinateSpace.World, colour, centre + new Vector2(40.0f, 10.0f), 32, 46.0f, 0.6f);
            draw.Helpers.DrawColouredPoly(stage, CoordinateSpace.World, colour, centre + new Vector2(85.0f, -5.0f), 32, 32.0f, 0.6f);
        }
    }

    private static void DrawMountain(IDrawing draw, IDrawStage stage, Vector2 baseCentre, float height, float widthScale, Colour colour, float depth)
    {
        // A regular triangle of 'radius' r has its top point r above the centre and its base r/2 below
        var radius = height / 1.5f;
        var centre = baseCentre + new Vector2(0.0f, 0.5f * radius);
        draw.Helpers.DrawColouredPoly(stage, CoordinateSpace.World, colour, centre, 3, radius, depth, 0, xScaling: widthScale);
    }
    #endregion
}
