using Microsoft.Xna.Framework;
using System;

namespace LanternHollow;

public class Guard
{
    public Vector2 Position;
    public int Facing;
    public Rectangle Bounds;

    private readonly Rectangle _platform;
    private const float Speed = Tuning.PatrolSpeed;
    private const int Width = Tuning.GuardWidth;
    private const int Height = Tuning.GuardHeight;

    public Guard(Rectangle platform)
    {
        _platform = platform;
        Position = new Vector2(platform.Left + platform.Width / 2f, platform.Top + 1);
        Facing = 1;
        Bounds = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - Height, Width, Height);
    }

    public void Update(float dt, Vector2 playerFeet)
    {
        float dx = playerFeet.X - Position.X;

        if (Math.Abs(dx) <= Tuning.SightRange)
        {
            float move = Tuning.ChaseSpeed * dt;

            if (Math.Abs(dx) > Tuning.ArriveDistance)
            {
                float newX = Position.X + Math.Sign(dx) * move;
                float le = newX - Width / 2f;
                float ri = newX + Width / 2f;

                if (le < _platform.Left)
                    newX = _platform.Left + Width / 2f;
                if (ri > _platform.Right)
                    newX = _platform.Right - Width / 2f;

                Facing = (int)Math.Sign(dx);
                Position = new Vector2(newX, Position.Y);
            }
        }
        else
        {
            float move = Speed * dt;
            float proposedX = Position.X + Facing * move;
            float le = proposedX - Width / 2f;
            float ri = proposedX + Width / 2f;
            bool atL = le < _platform.Left;
            bool atR = ri > _platform.Right;

            if (atL || atR)
            {
                Facing = -Facing;
                proposedX = Position.X + Facing * move;
                le = proposedX - Width / 2f;
                ri = proposedX + Width / 2f;

                if (atL)
                    proposedX = _platform.Left + Width / 2f;
                if (atR)
                    proposedX = _platform.Right - Width / 2f;
            }

            Position = new Vector2(proposedX, Position.Y);
        }

        Bounds = new Rectangle((int)Position.X - Width / 2, (int)Position.Y - Height, Width, Height);
    }
}
