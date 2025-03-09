using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformGame;

public class BackgroundLayer
{
    private Texture2D texture;
    private Vector2 position1;
    private Vector2 position2;
    private float scrollingSpeed;
    private float scale;
    private int screenWidth;

    public BackgroundLayer(Texture2D texture, float scrollingSpeed, float scale, int screenWidth)
    {
        this.texture = texture;
        this.scrollingSpeed = scrollingSpeed;
        this.scale = scale;
        this.screenWidth = screenWidth;
        
        // Set up initial positions
        position1 = Vector2.Zero;
        position2 = new Vector2(texture.Width * scale, 0);
    }

    public void Update(float cameraX)
    {
        // Move the background based on camera position and scrolling speed
        float parallaxScrolling = cameraX * scrollingSpeed;
        
        // Calculate the effective width of the texture when scaled
        float scaledTextureWidth = texture.Width * scale;

        // Update positions
        position1.X = -(parallaxScrolling % scaledTextureWidth);
        position2.X = position1.X + scaledTextureWidth;

        // If the first image is completely off screen to the right, move it to the left
        if (position1.X > screenWidth)
        {
            position1.X -= scaledTextureWidth * 2;
            position2.X -= scaledTextureWidth * 2;
        }
        // If the first image is completely off screen to the left, move it to the right
        else if (position1.X < -scaledTextureWidth)
        {
            position1.X += scaledTextureWidth * 2;
            position2.X += scaledTextureWidth * 2;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw both copies of the background
        spriteBatch.Draw(texture, position1, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(texture, position2, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }
}