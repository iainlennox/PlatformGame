using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformGame;

public class FruitPopup
{
    private string text;
    private Vector2 position;
    private float timeAlive;
    private const float DISPLAY_TIME = 1.0f;
    private const float FADE_START = 0.7f;
    private Vector2 velocity;
    private const float RISE_SPEED = -100f;

    public bool IsExpired => timeAlive >= DISPLAY_TIME;

    public FruitPopup(string fruitName, Vector2 position)
    {
        this.text = fruitName;
        this.position = position;
        this.timeAlive = 0;
        this.velocity = new Vector2(0, RISE_SPEED);
    }

    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        timeAlive += elapsed;
        position += velocity * elapsed;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Matrix? transform = null)
    {
        float alpha = 1.0f;
        if (timeAlive > FADE_START)
        {
            alpha = 1.0f - ((timeAlive - FADE_START) / (DISPLAY_TIME - FADE_START));
        }

        Color textColor = Color.White * alpha;
        Vector2 textSize = font.MeasureString(text);
        Vector2 textPosition = position - textSize / 2;

        if (transform.HasValue)
        {
            spriteBatch.Begin(transformMatrix: transform);
        }
        else
        {
            spriteBatch.Begin();
        }
        
        spriteBatch.DrawString(font, text, textPosition, textColor, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
        spriteBatch.End();
    }
}