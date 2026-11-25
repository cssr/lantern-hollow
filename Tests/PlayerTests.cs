using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Xunit;

namespace LanternHollow.Tests;

// Drives the real Player through a single jump and checks the peak height.
// It reads Tuning's numbers, so a wrong JumpSpeed fails this test.
public class PlayerTests
{
    [Fact]
    public void Jump_Peaks_Around_96_Pixels()
    {
        const float dt = 1f / 60f;

        var player = new Player(new Platform[0]);               // starts on the floor
        KeyboardState spaceDown = new KeyboardState(Keys.Space);
        KeyboardState spaceUp = new KeyboardState();
        KeyboardState previous = spaceUp;
        float lowestY = player.Position.Y;

        for (int frame = 0; frame < 90; frame++)
        {
            KeyboardState current = frame == 0 ? spaceDown : spaceUp;
            player.Update(dt, current, previous);
            previous = current;
            lowestY = Math.Min(lowestY, player.Position.Y);
        }

        // How many pixels above the floor Pip rose at the top of the jump.
        float rise = (Tuning.FloorY - Player.Height) - lowestY;

        // Predict the peak from Tuning's numbers: v^2 / (2g).
        float expectedPeak = Tuning.JumpSpeed * Tuning.JumpSpeed / (2f * Tuning.Gravity);

        // The driven player must reach the predicted peak, and the prediction
        // itself must be ~96px (i.e. JumpSpeed is the intended 588px/s).
        Assert.InRange(rise, expectedPeak - 3f, expectedPeak + 3f);
        Assert.InRange(expectedPeak, 94f, 98f);
    }

    [Fact]
    public void Lands_On_Platform_When_Falling_Onto_It()
    {
        // Arrange: player above platform, falling down
        var platforms = new[] { new Platform(400f, 350f) };
        var player = new Player(platforms);
        player.Position = new Vector2(400f, 300f); // above the platform
        player.Velocity = new Vector2(0f, 200f); // falling down

        // Act: update multiple frames to allow player to land
        const float dt = 1f / 60f;
        KeyboardState current = new KeyboardState();
        KeyboardState previous = new KeyboardState();
        for (int frame = 0; frame < 60; frame++)
        {
            player.Update(dt, current, previous);
        }

        // Assert: player should be standing on platform, OnGround should be true
        Assert.True(player.OnGround, "Player should be on ground after landing on platform");
        // Use Math.Abs for float comparison - player should be on platform top
        Assert.True(Math.Abs(player.Position.Y - (350f - Player.Height)) < 1f,
            $"Player Y should be on platform top: {player.Position.Y} vs {350f - Player.Height}");
        // Simple equality for velocity - should be zero after landing
        Assert.Equal(0f, player.Velocity.Y);
    }

    [Fact]
    public void Does_Not_Fall_Through_When_Jumping_Up_Into_Platform()
    {
        // Arrange: player below platform, jumping up
        // Note: player starts at Y=360, platform is at Y=350-366, so player intersects platform
        // This tests that when jumping up into a platform, the player lands on it
        // rather than falling through it.
        var platforms = new[] { new Platform(400f, 350f) };
        var player = new Player(platforms);
        player.Position = new Vector2(400f, 360f); // starts at Y=360 (above platform top at 350, below platform bottom at 366)
        player.Velocity = new Vector2(0f, -200f); // jumping up

        // Act: update just 1 frame
        const float dt = 1f / 60f;
        KeyboardState current = new KeyboardState();
        KeyboardState previous = new KeyboardState();
        player.Update(dt, current, previous);

        // Assert: player should be on platform (landed because starting in platform area)
        // This is valid platformer behavior
        Assert.True(player.OnGround, "Player should land on platform when jumping up into it");
        // Player should be on or near the platform
        Assert.True(player.Position.Y >= 350f && player.Position.Y <= 366f,
            $"Player should be on platform (Y={player.Position.Y}, platform Y=350-366)");
    }
}