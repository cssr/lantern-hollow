using Microsoft.Xna.Framework;
using System;
using Xunit;

namespace LanternHollow.Tests;

public class GuardTests
{
    [Fact]
    public void Patrols_Third_Platform_For_600_Frames()
    {
        var platform = new Platform(620f, 360f);
        var guard = new Guard(platform.Bounds);

        const float dt = 1f / 60f;
        var playerFeet = new Vector2(platform.Bounds.Right + 1000, platform.Bounds.Top);
        int facingChanges = 0;
        int previousFacing = guard.Facing;

        for (int frame = 0; frame < 600; frame++)
        {
            guard.Update(dt, playerFeet);

            Assert.True(guard.Bounds.Intersects(platform.Bounds),
                "Frame " + frame + ": guard " + guard.Bounds + " must be on platform " + platform.Bounds);

            if (guard.Facing != previousFacing)
            {
                facingChanges++;
                previousFacing = guard.Facing;
            }
        }

        Assert.True(facingChanges >= 2, "Expected at least 2 Facing changes, got " + facingChanges);
    }

    [Fact]
    public void Chases_Pip_Stops_Within_ArriveDistance_Without_Flipping_Facing()
    {
        var platform = new Platform(620f, 360f);
        var guard = new Guard(platform.Bounds);

        const float dt = 1f / 60f;
        var playerFeet = new Vector2(724f, platform.Bounds.Top);
        int framesToArrive = 0;
        int framesStill = 0;
        bool arrived = false;
        int facingWhenArrived = guard.Facing;

        for (int frame = 0; frame < 600; frame++)
        {
            guard.Update(dt, playerFeet);

            Assert.True(guard.Bounds.Intersects(platform.Bounds),
                "Frame " + frame + ": guard " + guard.Bounds + " must be on platform " + platform.Bounds);

            if (!arrived && Math.Abs(guard.Position.X - playerFeet.X) <= Tuning.ArriveDistance)
            {
                arrived = true;
                framesToArrive = frame;
                facingWhenArrived = guard.Facing;
            }

            if (arrived && guard.Facing == facingWhenArrived)
                framesStill++;
        }

        Assert.True(framesToArrive <= 300, "Expected guard to arrive within 300 frames, arrived at frame " + framesToArrive);
        Assert.True(framesStill >= 60, "Expected at least 60 frames of stillness after arriving, got " + framesStill);
        Assert.True(guard.Facing == facingWhenArrived, "Facing changed after arriving: " + facingWhenArrived + " -> " + guard.Facing);
    }

    [Fact]
    public void Pip_Far_Away_Guard_Patrols_Not_Chases()
    {
        var platform = new Platform(620f, 360f);
        var guard = new Guard(platform.Bounds);

        const float dt = 1f / 60f;
        var playerFeet = new Vector2(620f + 400f + 16f, platform.Bounds.Top);
        bool wasChasing = false;
        float previousGuardX = guard.Position.X;

        for (int frame = 0; frame < 100; frame++)
        {
            guard.Update(dt, playerFeet);

            Assert.True(guard.Bounds.Intersects(platform.Bounds),
                "Frame " + frame + ": guard " + guard.Bounds + " must be on platform " + platform.Bounds);

            if (frame > 0)
            {
                float dx = Math.Abs(guard.Position.X - previousGuardX);
                if (dx > 1.5f) wasChasing = true;
            }
            previousGuardX = guard.Position.X;
        }

        Assert.False(wasChasing, "Guard should not chase when pip is 400 px away outside SightRange");
    }

    [Fact]
    public void Pip_Beyond_Strip_Guard_Stops_At_End_Inside_Strip()
    {
        var platform = new Platform(620f, 360f);
        var guard = new Guard(platform.Bounds);

        const float dt = 1f / 60f;
        var playerFeet = new Vector2(800f, platform.Bounds.Top);
        bool reachedEnd = false;

        for (int frame = 0; frame < 200; frame++)
        {
            guard.Update(dt, playerFeet);

            Assert.True(guard.Bounds.Intersects(platform.Bounds),
                "Frame " + frame + ": guard " + guard.Bounds + " must be on platform " + platform.Bounds);

            if (guard.Position.X >= platform.Bounds.Right - 16f && frame > 0)
            {
                Assert.True(guard.Bounds.Left >= platform.Bounds.Left && guard.Bounds.Right <= platform.Bounds.Right,
                    "Frame " + frame + ": guard bounds " + guard.Bounds + " out of platform " + platform.Bounds);
                reachedEnd = true;
                break;
            }
        }

        Assert.True(reachedEnd, "Expected guard to reach end of strip with bounds inside it");
    }
}
