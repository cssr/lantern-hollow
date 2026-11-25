using Microsoft.Xna.Framework;
using Xunit;

namespace LanternHollow.Tests;

public class GuardTests
{
    [Fact]
    public void Patrols_Third_Platform_For_600_Frames()
    {
        // Third platform: the one at (620, 360) in Game1.cs
        // With Platform default width 128, it spans X: 620-748
        var platform = new Platform(620f, 360f);
        var guard = new Guard(platform.Bounds);

        const float dt = 1f / 60f;
        int facingChanges = 0;
        int previousFacing = guard.Facing;

        for (int frame = 0; frame < 600; frame++)
        {
            guard.Update(dt);

            // Guard must stay on platform (Bounds inside platform)
            Assert.True(guard.Bounds.Intersects(platform.Bounds),
                $"Frame {frame}: guard {guard.Bounds} must be on platform {platform.Bounds}");

            // Count Facing changes
            if (guard.Facing != previousFacing)
            {
                facingChanges++;
                previousFacing = guard.Facing;
            }
        }

        // After 600 frames at 60px/s on a 128px platform:
        // Crossing time = 128/60 ≈ 2.13s ≈ 128 frames
        // 600 frames ≈ 4.7 crossings, so Facing should have changed at least twice
        Assert.True(facingChanges >= 2, $"Expected at least 2 Facing changes, got {facingChanges}");
    }
}