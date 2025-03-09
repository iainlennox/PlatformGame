using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformGame;

public class Platform
{
    public Rectangle Bounds { get; private set; }
    private Texture2D texture;
    public Food Food { get; private set; }

    public Platform(Texture2D texture, Rectangle bounds, Food food = null)
    {
        this.texture = texture;
        this.Bounds = bounds;
        this.Food = food;
    }

    public void Update(GameTime gameTime)
    {
        Food?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Bounds, Color.White);
        Food?.Draw(spriteBatch);
    }
}