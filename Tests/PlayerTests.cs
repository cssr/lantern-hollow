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

        var player = new Player();               // starts on the floor
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
}