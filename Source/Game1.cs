using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LanternHollow;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private Texture2D _pipTexture;
    private KeyboardState _previousKeyboard;

    // Platforms the player can stand on.
    private readonly Platform[] _platforms;

    // Guard for the third platform.
    private Guard _guard;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Three platforms at the specified positions.
        _platforms = new Platform[]
        {
            new Platform(150f, 360f),
            new Platform(280f, 290f),
            new Platform(620f, 360f),
        };

        // Guard patrols the third platform.
        var first = _platforms[0].Bounds;
        var third = _platforms[2].Bounds;
        var walkArea = new Rectangle(first.Left, 440, third.Right - first.Left, 16);
        _guard = new Guard(walkArea);
    }

    protected override void Initialize()
    {
        _player = new Player(_platforms);
        _previousKeyboard = Keyboard.GetState();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pipTexture = CreateSolidTexture(Color.White, 2);
    }

    protected override void Update(GameTime gameTime)
    {
#if !__IOS__
        // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
        // (iOS does not allow programmatically closing an app.)
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
#endif

        KeyboardState keyboard = Keyboard.GetState();
        _player.Update((float)gameTime.ElapsedGameTime.TotalSeconds, keyboard, _previousKeyboard);
        _previousKeyboard = keyboard;
// Update guard AI
        var playerFeet = new Vector2(_player.Rectangle.X + _player.Rectangle.Width / 2f, _player.Rectangle.Y + _player.Rectangle.Height);
        _guard.Update((float)gameTime.ElapsedGameTime.TotalSeconds, playerFeet);

        // Check if Pip intersects guard - if so, take a hit
        if (_player.Rectangle.Intersects(_guard.Bounds))
        {
            _player.TakeHit();
        }

        // Also remove the player from the guard if they're not intersecting
        // (this prevents repeated hits while standing in the guard's area
        // after invulnerability expires - the guard will still detect intersection
        // but TakeHit will be no-op during invulnerability)

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // Draw Pip with invulnerability transparency
        Color drawColor = _player.IsInvulnerable ? Color.White * 0.5f : Color.White;
        _spriteBatch.Draw(_pipTexture, _player.Rectangle, drawColor);

        // Draw platforms as solid white rectangles.
        foreach (var platform in _platforms)
        {
            _spriteBatch.Draw(_pipTexture, platform.Bounds, Color.Gray);
        }

        // Draw guard bounds as teal rectangle.
        _spriteBatch.Draw(_pipTexture, _guard.Bounds, Color.Teal);

        // Draw health hearts at top left - one white square per heart
        for (int i = 0; i < _player.Health; i++)
        {
            _spriteBatch.Draw(_pipTexture, new Rectangle(10 + i * 10, 10, 8, 8), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    // Builds a small solid-color texture used to fill Pip's rectangle.
    private Texture2D CreateSolidTexture(Color color, int size)
    {
        var texture = new Texture2D(GraphicsDevice, size, size);
        var data = new Color[size * size];
        for (int i = 0; i < data.Length; i++)
            data[i] = color;
        texture.SetData(data);
        return texture;
    }
}