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

    public Player()
    {
        Position = new Vector2(400f, Tuning.FloorY - Height);
        Velocity = Vector2.Zero;
        OnGround = true;
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

        // Jump: only from the ground, and only on the press edge.
        if (current.IsKeyDown(Keys.Space) && !previous.IsKeyDown(Keys.Space) && OnGround)
        {
            velocity.Y = -Tuning.JumpSpeed;
            OnGround = false;
        }

        // Exact integration for constant acceleration (gravity).
        float nextVy = velocity.Y + Tuning.Gravity * deltaSeconds;
        Vector2 position = Position;
        // Horizontal motion: velocity is applied to the position this frame.
        position.X += velocity.X * deltaSeconds;
        position.Y += 0.5f * (velocity.Y + nextVy) * deltaSeconds;
        velocity.Y = nextVy;

        // Floor collision: clamp and stop vertical motion.
        if (position.Y + Height >= Tuning.FloorY)
        {
            position.Y = Tuning.FloorY - Height;
            velocity.Y = 0f;
            OnGround = true;
        }

        Position = position;
        Velocity = velocity;
    }
}