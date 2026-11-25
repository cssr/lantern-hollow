using Microsoft.Xna.Framework;

namespace LanternHollow;

// A static platform the player can stand on.
// Rectangle width = 128, height = 16.
// Position is the top-left corner.
public class Platform
{
    public readonly Rectangle Bounds;

    public Platform(float x, float y)
    {
        Bounds = new Rectangle((int)x, (int)y, 128, 16);
    }
}