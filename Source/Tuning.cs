namespace LanternHollow;

// All game numbers live here. Units: pixels and seconds. Y points down.
// With these values the jump peak is JumpSpeed^2 / (2 * Gravity) = 96 px.
// MaxHealth: starting hearts. InvulnerableTime: seconds of invincibility after a hit.
public static class Tuning
{
    public const float MoveSpeed = 220f;   // pixels per second
    public const float Gravity = 1800f;    // pixels per second, per second
    public const float JumpSpeed = 588f;   // pixels per second, upwards
    public const float FloorY = 440f;      // ground level; Pip rests above it
    public const float PatrolSpeed = 80f;   // pixels per second
    public const float SightRange = 20f;   // pixels - guard sees player within this distance
    public const float ChaseSpeed = 110f;   // pixels per second (chasing)
    public const float ArriveDistance = 12f;   // pixels - stop when within this distance of player's x
    public const int GuardWidth = 32;       // guard rectangle width
    public const int GuardHeight = 48;      // guard rectangle height
    public const int MaxHealth = 3;
    public const float InvulnerableTime = 1f;
}
