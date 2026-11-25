using Microsoft.Xna.Framework;

namespace LanternHollow;

// A guard that walks left and right along the top of a platform.
public class Guard
{
    // Centre x, feet y.
    public Vector2 Position;

    // 1 = right, -1 = left.
    public int Facing;

    public Rectangle Bounds;

    // Platform the guard patrols.
    private readonly Rectangle _platform;

    // How fast the guard walks, in pixels per second.
    private const float Speed = Tuning.PatrolSpeed;
    private const int Width = Tuning.GuardWidth;
    private const int Height = Tuning.GuardHeight;

    public Guard(Rectangle platform)
    {
        _platform = platform;

        // Start in the middle of the platform top.
        Position = new Vector2(
            platform.Left + platform.Width / 2f,
            platform.Top);

        Facing = 1; // start facing right
        Bounds = new Rectangle(
            (int)Position.X - Width / 2,
            (int)Position.Y - Height,
            Width,
            Height);
    }

    public void Update(float dt)
    {
        // How far we want to move this frame.
        float move = Speed * dt;

        // Proposed new centre x.
        float proposedX = Position.X + Facing * move;

        // Check if the guard would walk off the platform.
        float leftEdge = proposedX - Width / 2f;
        float rightEdge = proposedX + Width / 2f;

        bool atLeftEdge = leftEdge < _platform.Left;
        bool atRightEdge = rightEdge > _platform.Right;

        if (atLeftEdge || atRightEdge)
        {
            // Turn around.
            Facing = -Facing;

            // Recalculate proposed position with new direction.
            proposedX = Position.X + Facing * move;
            leftEdge = proposedX - Width / 2f;
            rightEdge = proposedX + Width / 2f;

            // Clamp to platform boundaries so we don't stick out.
            if (atLeftEdge)
                proposedX = _platform.Left + Width / 2f;
            if (atRightEdge)
                proposedX = _platform.Right - Width / 2f;

            Position = new Vector2(proposedX, Position.Y);
        }
        else
        {
            // Safe to move.
            Position = new Vector2(proposedX, Position.Y);
        }

        // Update bounds.
        Bounds = new Rectangle(
            (int)Position.X - Width / 2,
            (int)Position.Y - Height,
            Width,
            Height);
    }
}