using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LanternHollow;

// Pip, the player character.
// Horizontal movement plus grounded jumping, driven by Tuning's constants.
// Y points down, so gravity is positive and a jump velocity is negative (up).
public class Player
{
    // Placeholder sprite size; draw Pip as a 32 x 48 rectangle for now.
    public const float Width = 32f;
    public const float Height = 48f;

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public bool OnGround { get; set; }

    // Platforms the player can stand on.
    private readonly Platform[] _platforms;

    // Track player position from last frame for landing detection
    private Vector2 _lastPosition;

    public Player(Platform[] platforms)
    {
        Position = new Vector2(400f, Tuning.FloorY - Height);
        Velocity = Vector2.Zero;
        OnGround = true;
        _platforms = platforms;
        _lastPosition = Position;
    }

    public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height);

    public void Update(float deltaSeconds, KeyboardState current, KeyboardState previous)
    {
        // Horizontal input -> target velocity.
        float direction = 0f;
        // Arrow keys and A/D both move left; arrows and D both move right.
        if (current.IsKeyDown(Keys.Left)) direction -= 1f;
        if (current.IsKeyDown(Keys.A)) direction -= 1f;
        if (current.IsKeyDown(Keys.Right)) direction += 1f;
        if (current.IsKeyDown(Keys.D)) direction += 1f;

        // Vector2 is a struct, so mutate a local copy and assign it back.
        Vector2 velocity = Velocity;
        velocity.X = direction * Tuning.MoveSpeed;
        Vector2 position = Position;

        // Jump: only from the ground, and only on the press edge.
        if (current.IsKeyDown(Keys.Space) && !previous.IsKeyDown(Keys.Space) && OnGround)
        {
            velocity.Y = -Tuning.JumpSpeed;
            OnGround = false;
        }

        // --- Horizontal motion first ---
        // Apply horizontal velocity using a local position copy.
        position.X += velocity.X * deltaSeconds;

        // Separate X collision: if we overlap a platform horizontally, push out.
        Rectangle newRect = new Rectangle((int)position.X, (int)position.Y, (int)Width, (int)Height);
        foreach (var platform in _platforms)
        {
            if (!newRect.Intersects(platform.Bounds)) continue;

            // Push out horizontally
            if (velocity.X > 0) // moving right
            {
                position.X = platform.Bounds.Left - Width;
            }
            else if (velocity.X < 0) // moving left
            {
                position.X = platform.Bounds.Right;
            }
            break;
        }

        // --- Vertical motion ---
        // Exact integration for constant acceleration (gravity).
        float nextVy = velocity.Y + Tuning.Gravity * deltaSeconds;
        position.Y += 0.5f * (velocity.Y + nextVy) * deltaSeconds;
        velocity.Y = nextVy;

        // Check platform collision for vertical movement
        // We'll handle landing directly without relying on penetration side detection
        Rectangle futureRect = new Rectangle((int)position.X, (int)position.Y, (int)Width, (int)Height);

        // Check intersection with each platform and handle landing
        foreach (var platform in _platforms)
        {
            if (!futureRect.Intersects(platform.Bounds)) continue;

            // Check if landing on top: player is falling and feet were above platform last frame
            if (velocity.Y > 0 && _lastPosition.Y + Height <= platform.Bounds.Top)
            {
                // Land on platform
                position.Y = platform.Bounds.Top - Height;
                velocity.Y = 0f;
                OnGround = true;
            }
            // Check if hitting head from below: player is jumping up and head hits platform bottom
            else if (velocity.Y < 0 && _lastPosition.Y >= platform.Bounds.Bottom)
            {
                // Hit head on platform
                position.Y = platform.Bounds.Bottom;
                velocity.Y = 0f;
            }
            // Horizontal collision - push out
            else if (velocity.X > 0 && position.X < platform.Bounds.Left)
            {
                // Moving right into platform
                position.X = platform.Bounds.Left - Width;
            }
            else if (velocity.X < 0 && position.X + Width > platform.Bounds.Right)
            {
                // Moving left into platform
                position.X = platform.Bounds.Right;
            }
        }

        // Floor collision: clamp and stop vertical motion.
        if (position.Y + Height >= Tuning.FloorY)
        {
            position.Y = Tuning.FloorY - Height;
            velocity.Y = 0f;
            OnGround = true;
        }

        // Store current position as last position for next frame
        _lastPosition = Position;

        Position = position;
        Velocity = velocity;
    }
}