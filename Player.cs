using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace PlatformGame;

public class Player
{
    // Position and physics properties
    private Vector2 position;
    private Vector2 velocity;
    private Rectangle bounds;
    private bool isJumping;
    private int jumpsRemaining;
    private float jumpForce = -12f;
    private float doubleJumpForce = -15f;
    private float gravity = 0.5f;
    private float moveSpeed = 5f;
    private float minSpeed = 3f;
    private KeyboardState previousKeyboardState;

    // Animation properties
    private float runningTime;
    private const float ANIMATION_SPEED = 0.1f;
    private int currentFrame;
    private const int TOTAL_FRAMES = 4;
    private bool isMovingLegs = false;
    private float armRotation;
    private float legRotation;
    private const float MAX_LIMB_ROTATION = 0.4f;
    
    // Body part textures
    private Texture2D bodyTexture;
    private Texture2D armTexture;
    private Texture2D legTexture;
    
    // Part positions relative to body
    private Vector2 leftArmPosition;
    private Vector2 rightArmPosition;
    private Vector2 leftLegPosition;
    private Vector2 rightLegPosition;
    private Vector2 bodyOffset;

    public Vector2 Position => position;

    public Player(Texture2D bodyTex, Texture2D armTex, Texture2D legTex, Vector2 startPosition)
    {
        this.bodyTexture = bodyTex;
        this.armTexture = armTex;
        this.legTexture = legTex;
        this.position = startPosition;
        this.velocity = Vector2.Zero;
        
        // Set up body parts positions
        bodyOffset = new Vector2(0, -bodyTexture.Height / 2);
        leftArmPosition = new Vector2(-bodyTexture.Width / 2, -bodyTexture.Height / 4);
        rightArmPosition = new Vector2(bodyTexture.Width / 2, -bodyTexture.Height / 4);
        leftLegPosition = new Vector2(-bodyTexture.Width / 4, bodyTexture.Height / 2);
        rightLegPosition = new Vector2(bodyTexture.Width / 4, bodyTexture.Height / 2);

        this.bounds = new Rectangle(
            (int)position.X - bodyTexture.Width / 2,
            (int)position.Y - bodyTexture.Height / 2,
            bodyTexture.Width,
            bodyTexture.Height
        );

        this.isJumping = false;
        this.jumpsRemaining = 2;
        this.previousKeyboardState = Keyboard.GetState();
        this.runningTime = 0;
        this.currentFrame = 0;
    }

    public void Update(GameTime gameTime, List<Platform> platforms)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        
        // Horizontal movement
        velocity.X = minSpeed;
        if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            velocity.X = moveSpeed;

        // Jumping
        bool jumpKeyPressed = (keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.W)) &&
                            (!previousKeyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.W));

        if (jumpKeyPressed && jumpsRemaining > 0)
        {
            velocity.Y = (jumpsRemaining == 2) ? jumpForce : doubleJumpForce;
            jumpsRemaining--;
            isJumping = true;
        }

        // Apply gravity
        velocity.Y += gravity;

        // Update animation
        runningTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (runningTime >= ANIMATION_SPEED)
        {
            currentFrame = (currentFrame + 1) % TOTAL_FRAMES;
            runningTime = 0;
            isMovingLegs = !isMovingLegs;
        }

        // Update limb rotations based on movement
        float animationProgress = (float)currentFrame / TOTAL_FRAMES;
        if (isMovingLegs)
        {
            legRotation = MathHelper.Lerp(-MAX_LIMB_ROTATION, MAX_LIMB_ROTATION, animationProgress);
            armRotation = -legRotation; // Arms move opposite to legs
        }
        else
        {
            legRotation = MathHelper.Lerp(MAX_LIMB_ROTATION, -MAX_LIMB_ROTATION, animationProgress);
            armRotation = -legRotation;
        }

        // If jumping, adjust limb positions accordingly
        if (isJumping)
        {
            legRotation = MAX_LIMB_ROTATION;
            armRotation = -MAX_LIMB_ROTATION * 0.5f;
        }

        // Update position and check collisions
        Vector2 nextPosition = position + velocity;
        Rectangle nextBounds = new Rectangle(
            (int)nextPosition.X - bodyTexture.Width / 2,
            (int)nextPosition.Y - bodyTexture.Height / 2,
            bodyTexture.Width,
            bodyTexture.Height
        );

        bool hasCollision = false;
        foreach (var platform in platforms)
        {
            if (nextBounds.Intersects(platform.Bounds))
            {
                if (velocity.Y > 0 && bounds.Bottom <= platform.Bounds.Top)
                {
                    position.Y = platform.Bounds.Top - bounds.Height / 2;
                    velocity.Y = 0;
                    isJumping = false;
                    jumpsRemaining = 2;
                    hasCollision = true;
                }
                else if (velocity.Y < 0 && bounds.Top >= platform.Bounds.Bottom)
                {
                    position.Y = platform.Bounds.Bottom + bounds.Height / 2;
                    velocity.Y = 0;
                    hasCollision = true;
                }
                
                if (velocity.X > 0 && bounds.Right <= platform.Bounds.Left)
                {
                    position.X = platform.Bounds.Left - bounds.Width / 2;
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
        bounds.X = (int)position.X - bodyTexture.Width / 2;
        bounds.Y = (int)position.Y - bodyTexture.Height / 2;

        previousKeyboardState = keyboardState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw body
        spriteBatch.Draw(bodyTexture, position + bodyOffset, null, Color.White, 0f,
            new Vector2(bodyTexture.Width / 2, bodyTexture.Height / 2), 1f, SpriteEffects.None, 0f);

        // Draw arms with rotation
        spriteBatch.Draw(armTexture, position + leftArmPosition, null, Color.White, armRotation,
            new Vector2(armTexture.Width / 2, 0), 1f, SpriteEffects.None, 0f);
        spriteBatch.Draw(armTexture, position + rightArmPosition, null, Color.White, -armRotation,
            new Vector2(armTexture.Width / 2, 0), 1f, SpriteEffects.FlipHorizontally, 0f);

        // Draw legs with rotation
        spriteBatch.Draw(legTexture, position + leftLegPosition, null, Color.White, legRotation,
            new Vector2(legTexture.Width / 2, 0), 1f, SpriteEffects.None, 0f);
        spriteBatch.Draw(legTexture, position + rightLegPosition, null, Color.White, -legRotation,
            new Vector2(legTexture.Width / 2, 0), 1f, SpriteEffects.FlipHorizontally, 0f);
    }
}