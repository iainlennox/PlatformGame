using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace PlatformGame;

public class Player
{
    private Vector2 position;
    private Vector2 velocity;
    private Rectangle bounds;
    private bool isJumping;
    private int jumpsRemaining; // Track available jumps
    private float jumpForce = -12f;
    private float doubleJumpForce = -15f; // Stronger second jump
    private float gravity = 0.5f;
    private float moveSpeed = 5f;
    private Texture2D texture;
    private float minSpeed = 3f;
    private KeyboardState previousKeyboardState; // To track key press transitions
    public Vector2 Position => position;

    public Player(Texture2D texture, Vector2 position)
    {
        this.texture = texture;
        this.position = position;
        this.velocity = Vector2.Zero;
        this.bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        this.isJumping = false;
        this.jumpsRemaining = 2; // Start with 2 jumps available
        this.previousKeyboardState = Keyboard.GetState();
    }

    public void Update(GameTime gameTime, List<Platform> platforms)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        
        // Horizontal movement - always move forward but allow speed boost
        velocity.X = minSpeed;
        if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            velocity.X = moveSpeed;

        // Jumping
        bool jumpKeyPressed = (keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.W)) &&
                            (!previousKeyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.W));

        if (jumpKeyPressed && jumpsRemaining > 0)
        {
            // Apply different jump force based on whether it's the first or second jump
            velocity.Y = (jumpsRemaining == 2) ? jumpForce : doubleJumpForce;
            jumpsRemaining--;
            isJumping = true;
        }

        // Apply gravity
        velocity.Y += gravity;

        // Update position and check collisions
        Vector2 nextPosition = position + velocity;
        Rectangle nextBounds = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, bounds.Width, bounds.Height);

        bool hasCollision = false;
        foreach (var platform in platforms)
        {
            if (nextBounds.Intersects(platform.Bounds))
            {
                // Handle collision
                if (velocity.Y > 0 && bounds.Bottom <= platform.Bounds.Top) // Landing on platform
                {
                    position.Y = platform.Bounds.Top - bounds.Height;
                    velocity.Y = 0;
                    isJumping = false;
                    jumpsRemaining = 2; // Reset jumps when landing
                    hasCollision = true;
                }
                else if (velocity.Y < 0 && bounds.Top >= platform.Bounds.Bottom) // Hitting head
                {
                    position.Y = platform.Bounds.Bottom;
                    velocity.Y = 0;
                    hasCollision = true;
                }
                
                // Only check horizontal collision for right side since we're always moving right
                if (velocity.X > 0 && bounds.Right <= platform.Bounds.Left) // Right collision
                {
                    position.X = platform.Bounds.Left - bounds.Width;
                    velocity.X = 0;
                    hasCollision = true;
                }
            }
        }

        if (!hasCollision)
        {
            position = nextPosition;
        }

        // Update bounds
        bounds.X = (int)position.X;
        bounds.Y = (int)position.Y;

        // Store current keyboard state for next frame
        previousKeyboardState = keyboardState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, Color.White);
    }
}