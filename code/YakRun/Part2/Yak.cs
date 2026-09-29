using System.Numerics;
using Yak2D;

namespace YakRun;

public class Yak
{
    #region constants
    // The collision box. The sprite is drawn a little larger than this
    public const float Width = 64.0f;
    public const float Height = 60.0f;

    private const float SpriteWidth = 84.0f;
    private const float SpriteHeight = SpriteWidth * (228.0f / 256.0f); // keep the texture's aspect ratio

    private const float RunSpeed = 380.0f;       // units per second
    private const float Acceleration = 2600.0f;  // units per second, per second
    private const float Gravity = 2400.0f;
    private const float JumpSpeed = 960.0f;
    private const float MaxFallSpeed = 1500.0f;
    #endregion

    #region state
    public Vector2 Position;   // centre of the collision box, in world units
    public Vector2 Velocity;
    public bool OnGround { get; private set; }

    private float _runCycle;   // advances while running, drives the bobbing animation

    public Box Bounds => Box.FromCentre(Position, Width, Height);

    public Yak(Vector2 start)
    {
        Position = start;
    }
    #endregion

    #region update
    // moveInput is -1 (left) to +1 (right). Called once per fixed update step
    public void Update(float seconds, float moveInput, bool jumpPressed, IReadOnlyList<Box> platforms)
    {
        // Ease towards the target running speed, so starting and stopping feel smooth
        Velocity.X = MoveTowards(Velocity.X, moveInput * RunSpeed, Acceleration * seconds);

        if (jumpPressed && OnGround)
        {
            Velocity.Y = JumpSpeed;
        }

        Velocity.Y = MathF.Max(Velocity.Y - (Gravity * seconds), -MaxFallSpeed);

        // Move one axis at a time, pushing the yak back out of anything it now overlaps
        Position.X += Velocity.X * seconds;
        foreach (var platform in platforms)
        {
            if (Bounds.Overlaps(platform))
            {
                Position.X = Velocity.X > 0.0f ? platform.Left - (0.5f * Width) : platform.Right + (0.5f * Width);
                Velocity.X = 0.0f;
            }
        }

        Position.Y += Velocity.Y * seconds;
        OnGround = false;
        foreach (var platform in platforms)
        {
            if (Bounds.Overlaps(platform))
            {
                if (Velocity.Y < 0.0f)
                {
                    Position.Y = platform.Top + (0.5f * Height); // landed on top
                    OnGround = true;
                }
                else
                {
                    Position.Y = platform.Bottom - (0.5f * Height); // bumped our head
                }
                Velocity.Y = 0.0f;
            }
        }

        var running = OnGround && MathF.Abs(Velocity.X) > 10.0f;
        _runCycle = running ? _runCycle + (seconds * 16.0f) : 0.0f;
    }

    private static float MoveTowards(float current, float target, float maxChange)
    {
        var difference = target - current;
        return MathF.Abs(difference) <= maxChange ? target : current + (MathF.Sign(difference) * maxChange);
    }
    #endregion

    #region draw
    public void Draw(IDrawing draw, IDrawStage stage, ITexture texture)
    {
        // A little squash and stretch: bob while running, stretch upwards while in the air
        var bob = MathF.Sin(_runCycle);
        var scaleX = OnGround ? 1.0f + (0.05f * bob) : 0.9f;
        var scaleY = OnGround ? 1.0f - (0.07f * bob) : 1.12f;

        var width = SpriteWidth * scaleX;
        var height = SpriteHeight * scaleY;

        // Keep the sprite's feet on the bottom of the collision box whatever its height
        var centre = new Vector2(Position.X, Position.Y - (0.5f * Height) + (0.5f * height));

        // Lean in the direction of travel (rotation is clockwise, in radians)
        var lean = 0.15f * (Velocity.X / RunSpeed);

        draw.Helpers.DrawTexturedQuad(stage,
                                      CoordinateSpace.World,
                                      texture,
                                      Colour.White,
                                      centre,
                                      width,
                                      height,
                                      depth: 0.5f,
                                      layer: 2,
                                      rotation_clockwise_radians: lean);
    }
    #endregion
}
