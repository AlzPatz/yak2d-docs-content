using System.Numerics;

namespace YakRun;

// An axis-aligned rectangle in world units. World space has +Y pointing up,
// so Bottom is the smaller Y value and Top the larger
public readonly record struct Box(float Left, float Bottom, float Width, float Height)
{
    public float Right => Left + Width;
    public float Top => Bottom + Height;
    public Vector2 Centre => new Vector2(Left + (0.5f * Width), Bottom + (0.5f * Height));

    public static Box FromCentre(Vector2 centre, float width, float height) =>
        new Box(centre.X - (0.5f * width), centre.Y - (0.5f * height), width, height);

    // Strict inequalities, so boxes that are only touching (e.g. standing on the ground) do not overlap
    public bool Overlaps(Box other) =>
        Left < other.Right && Right > other.Left && Bottom < other.Top && Top > other.Bottom;
}
