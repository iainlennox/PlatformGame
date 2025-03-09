using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PlatformGame;

public class Food
{
    public Rectangle Bounds { get; private set; }
    private Texture2D texture;
    private Vector2 position;
    private bool isCollected;
    private float bounceTime;
    private const float BOUNCE_SPEED = 2f;
    private const float BOUNCE_HEIGHT = 10f;
    private Vector2 originalPosition;
    public FoodType Type { get; private set; }
    private float rotation;
    private const float ROTATION_SPEED = 2f;

    public bool IsCollected => isCollected;

    public Food(Texture2D texture, Vector2 position, FoodType type)
    {
        this.texture = texture;
        this.position = position;
        this.originalPosition = position;
        this.Type = type;
        this.isCollected = false;
        this.bounceTime = 0;
        this.rotation = 0;
        this.Bounds = new Rectangle(
            (int)position.X - texture.Width / 2,
            (int)position.Y - texture.Height / 2,
            texture.Width,
            texture.Height
        );
    }

    public void Update(GameTime gameTime)
    {
        if (!isCollected)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Make the food bounce up and down
            bounceTime += elapsed * BOUNCE_SPEED;
            position.Y = originalPosition.Y + (float)Math.Sin(bounceTime) * BOUNCE_HEIGHT;
            
            // Rotate the food
            rotation += elapsed * ROTATION_SPEED;
            
            // Update bounds with new position
            Bounds = new Rectangle(
                (int)position.X - texture.Width / 2,
                (int)position.Y - texture.Height / 2,
                texture.Width,
                texture.Height
            );
        }
    }

    public void Collect()
    {
        isCollected = true;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!isCollected)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation,
                new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}