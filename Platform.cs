using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformGame;

public class Platform
{
    public Rectangle Bounds { get; private set; }
    private Texture2D texture;

    public Platform(Texture2D texture, Rectangle bounds)
    {
        this.texture = texture;
        this.Bounds = bounds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Bounds, Color.White);
    }
}