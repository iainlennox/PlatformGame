using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace PlatformGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private Texture2D _playerBodyTexture;
    private Texture2D _playerArmTexture;
    private Texture2D _playerLegTexture;
    private Texture2D _playerHeadTexture;
    private Texture2D _platformTexture;
    private List<Platform> _platforms;
    private Matrix _camera;
    private Random _random;
    private float _lastPlatformX;
    private const float MIN_PLATFORM_GAP = 100f;
    private const float MAX_PLATFORM_GAP = 250f;
    private const float PLATFORM_Y_VARIATION = 100f;
    private float _baseY = 300f;
    private SpriteFont _scoreFont;
    private int _score;
    private float _timeSinceStart;
    private const float SCORE_INCREMENT_INTERVAL = 1f; // Increment score every second
    private const float DEATH_Y_THRESHOLD = 800f; // Y position that triggers death
    private GameState _currentGameState;
    private KeyboardState _previousKeyboardState;
    private string _titleText = "NOAH'S GAME";
    private string _startPrompt = "Press SPACE to Start";
    private int _highScore;
    private string _gameOverText = "GAME OVER";
    private string _finalScoreText = "Final Score: ";
    private string _highScoreText = "High Score: ";
    private string _restartPrompt = "Press SPACE to Play Again";
    private string _quitPrompt = "Press ESC to Quit";
    private List<BackgroundLayer> _backgroundLayers;
    private Texture2D _skyTexture;
    private Texture2D _mountainsTexture;
    private Texture2D _cloudsTexture;
    private Texture2D _foodTexture;
    private Texture2D _orangeTexture;
    private Texture2D _bananaTexture;
    private const int FOOD_POINTS = 10;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _platforms = new List<Platform>();
        _backgroundLayers = new List<BackgroundLayer>();
        _random = new Random();
        _score = 0;
        _highScore = 0;
        _timeSinceStart = 0;
        _currentGameState = GameState.TitleScreen;
        Window.Title = "Noah's Game";
    }

    protected override void Initialize()
    {
        // Create player body texture (blue rectangle)
        _playerBodyTexture = new Texture2D(GraphicsDevice, 20, 40);
        Color[] bodyColorData = new Color[20 * 40];
        for (int i = 0; i < bodyColorData.Length; i++)
            bodyColorData[i] = Color.Blue;
        _playerBodyTexture.SetData(bodyColorData);

        // Create player arm texture (red rectangle)
        _playerArmTexture = new Texture2D(GraphicsDevice, 8, 20);
        Color[] armColorData = new Color[8 * 20];
        for (int i = 0; i < armColorData.Length; i++)
            armColorData[i] = Color.Red;
        _playerArmTexture.SetData(armColorData);

        // Create player leg texture (yellow rectangle)
        _playerLegTexture = new Texture2D(GraphicsDevice, 8, 25);
        Color[] legColorData = new Color[8 * 25];
        for (int i = 0; i < legColorData.Length; i++)
            legColorData[i] = Color.Yellow;
        _playerLegTexture.SetData(legColorData);

        // Create platform texture
        _platformTexture = new Texture2D(GraphicsDevice, 1, 1);
        Color[] platformColorData = new Color[1];
        platformColorData[0] = Color.Green;
        _platformTexture.SetData(platformColorData);

        // Create food texture (yellow circle)
        _foodTexture = new Texture2D(GraphicsDevice, 16, 16);
        Color[] foodColorData = new Color[16 * 16];
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                float distanceFromCenter = (float)Math.Sqrt(Math.Pow(x - 8, 2) + Math.Pow(y - 8, 2));
                if (distanceFromCenter <= 8)
                {
                    foodColorData[y * 16 + x] = Color.Yellow;
                }
                else
                {
                    foodColorData[y * 16 + x] = Color.Transparent;
                }
            }
        }
        _foodTexture.SetData(foodColorData);

        // Create orange texture (orange circle)
        _orangeTexture = new Texture2D(GraphicsDevice, 20, 20);
        Color[] orangeData = new Color[20 * 20];
        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                float distanceFromCenter = (float)Math.Sqrt(Math.Pow(x - 10, 2) + Math.Pow(y - 10, 2));
                if (distanceFromCenter <= 10)
                {
                    orangeData[y * 20 + x] = Color.Orange;
                }
                else
                {
                    orangeData[y * 20 + x] = Color.Transparent;
                }
            }
        }
        _orangeTexture.SetData(orangeData);

        // Create banana texture (yellow curved rectangle)
        _bananaTexture = new Texture2D(GraphicsDevice, 24, 16);
        Color[] bananaData = new Color[24 * 16];
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 24; x++)
            {
                float curve = 4 * (float)Math.Sin(x * Math.PI / 24);
                float distanceFromCurve = Math.Abs(y - (8 + curve));
                if (distanceFromCurve < 4)
                {
                    bananaData[y * 24 + x] = Color.Yellow;
                }
                else
                {
                    bananaData[y * 24 + x] = Color.Transparent;
                }
            }
        }
        _bananaTexture.SetData(bananaData);

        // Create the player with all body parts
        _player = new Player(_playerBodyTexture, _playerArmTexture, _playerLegTexture, _playerHeadTexture, new Vector2(100, 100));

        // Create initial platforms
        _lastPlatformX = 50;
        GenerateInitialPlatforms();

        // Create background textures
        _skyTexture = new Texture2D(GraphicsDevice, 1, 1);
        _skyTexture.SetData(new[] { Color.LightSkyBlue });

        _mountainsTexture = new Texture2D(GraphicsDevice, 800, 200);
        Color[] mountainData = new Color[800 * 200];
        for (int y = 0; y < 200; y++)
        {
            for (int x = 0; x < 800; x++)
            {
                // Create a mountain silhouette effect
                float height = 100 + (float)(Math.Sin(x * 0.02) * 50) + (float)(Math.Sin(x * 0.05) * 30);
                mountainData[y * 800 + x] = y > height ? Color.DarkSlateGray : Color.Transparent;
            }
        }
        _mountainsTexture.SetData(mountainData);

        _cloudsTexture = new Texture2D(GraphicsDevice, 800, 100);
        Color[] cloudData = new Color[800 * 100];
        for (int i = 0; i < cloudData.Length; i++)
        {
            // Create some simple cloud shapes
            float noise = (float)_random.NextDouble();
            cloudData[i] = noise > 0.95f ? new Color(1f, 1f, 1f, 0.3f) : Color.Transparent;
        }
        _cloudsTexture.SetData(cloudData);

        // Create background layers with different scroll speeds
        _backgroundLayers = new List<BackgroundLayer>
        {
            new BackgroundLayer(_skyTexture, 0f, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Width),
            new BackgroundLayer(_mountainsTexture, 0.2f, 1f, GraphicsDevice.Viewport.Width),
            new BackgroundLayer(_cloudsTexture, 0.1f, 2f, GraphicsDevice.Viewport.Width)
        };

        base.Initialize();
    }

    private void GenerateInitialPlatforms()
    {
        // Create starting platform
        _platforms.Add(new Platform(_platformTexture, new Rectangle(50, 400, 200, 20)));
        _lastPlatformX = 250;

        // Generate a few platforms ahead
        for (int i = 0; i < 5; i++)
        {
            GenerateNextPlatform();
        }
    }

    private Food CreateFoodForPlatform(Rectangle platformBounds)
    {
        if (_random.Next(2) == 0) // 50% chance to spawn food
        {
            Vector2 foodPosition = new Vector2(
                platformBounds.Center.X,
                platformBounds.Top - 20 // Position above platform
            );

            // Randomly choose between orange and banana
            FoodType foodType = (FoodType)_random.Next(2);
            Texture2D foodTexture = foodType == FoodType.Orange ? _orangeTexture : _bananaTexture;
            
            return new Food(foodTexture, foodPosition, foodType);
        }
        return null;
    }

    private void GenerateNextPlatform()
    {
        float gap = _random.NextSingle() * (MAX_PLATFORM_GAP - MIN_PLATFORM_GAP) + MIN_PLATFORM_GAP;
        float platformX = _lastPlatformX + gap;
        float platformY = _baseY + (_random.NextSingle() * 2 - 1) * PLATFORM_Y_VARIATION;
        int platformWidth = _random.Next(100, 200);
        
        Rectangle platformBounds = new Rectangle((int)platformX, (int)platformY, platformWidth, 20);
        Food food = CreateFoodForPlatform(platformBounds);
        
        _platforms.Add(new Platform(_platformTexture, platformBounds, food));
        _lastPlatformX = platformX + platformWidth;
    }

    private void CheckFoodCollection()
    {
        Rectangle playerBounds = new Rectangle(
            (int)_player.Position.X - _playerBodyTexture.Width / 2,
            (int)_player.Position.Y - _playerBodyTexture.Height / 2,
            _playerBodyTexture.Width,
            _playerBodyTexture.Height
        );

        foreach (var platform in _platforms)
        {
            if (platform.Food != null && !platform.Food.IsCollected && 
                playerBounds.Intersects(platform.Food.Bounds))
            {
                platform.Food.Collect();
                _score += FOOD_POINTS;
            }
        }
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Load the head texture
        _playerHeadTexture = Content.Load<Texture2D>("Noah");
        
        // Create a basic font texture for score display
        _scoreFont = Content.Load<SpriteFont>("Score");
    }

    private void ResetGame()
    {
        // Update high score before resetting score
        if (_score > _highScore)
        {
            _highScore = _score;
        }
        
        _score = 0;
        _timeSinceStart = 0;
        _lastPlatformX = 50;
        _platforms.Clear();
        GenerateInitialPlatforms();
        _player = new Player(_playerBodyTexture, _playerArmTexture, _playerLegTexture, _playerHeadTexture, new Vector2(100, 100));
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();

        if (currentKeyboardState.IsKeyDown(Keys.Escape))
        {
            if (_currentGameState == GameState.GameOver)
            {
                Exit();
            }
            else if (!_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                _currentGameState = GameState.GameOver;
            }
        }

        switch (_currentGameState)
        {
            case GameState.TitleScreen:
                if (currentKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
                {
                    _currentGameState = GameState.Playing;
                    ResetGame();
                }
                break;

            case GameState.Playing:
                // Check for death condition
                if (_player.Position.Y > DEATH_Y_THRESHOLD)
                {
                    _currentGameState = GameState.GameOver;
                    return;
                }

                // Update score based on time
                _timeSinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _score = (int)(_timeSinceStart);

                _player.Update(gameTime, _platforms);

                // Update camera to follow player
                _camera = Matrix.CreateTranslation(
                    -_player.Position.X + GraphicsDevice.Viewport.Width * 0.3f,
                    0,
                    0);

                // Remove platforms that are too far behind
                _platforms.RemoveAll(p => p.Bounds.Right < _player.Position.X - GraphicsDevice.Viewport.Width * 0.5f);

                // Generate new platforms ahead
                while (_lastPlatformX < _player.Position.X + GraphicsDevice.Viewport.Width * 1.5f)
                {
                    GenerateNextPlatform();
                }

                // Update background layers
                float cameraX = _player.Position.X - GraphicsDevice.Viewport.Width * 0.3f;
                foreach (var layer in _backgroundLayers)
                {
                    layer.Update(cameraX);
                }

                // Update platforms and check for food collection
                foreach (var platform in _platforms)
                {
                    platform.Update(gameTime);
                }
                CheckFoodCollection();

                break;

            case GameState.GameOver:
                if (currentKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
                {
                    _currentGameState = GameState.Playing;
                    ResetGame();
                }
                break;
        }

        _previousKeyboardState = currentKeyboardState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        switch (_currentGameState)
        {
            case GameState.TitleScreen:
                _spriteBatch.Begin();
                
                // Draw title text centered
                Vector2 titleSize = _scoreFont.MeasureString(_titleText);
                Vector2 titlePosition = new Vector2(
                    (GraphicsDevice.Viewport.Width - titleSize.X) / 2,
                    GraphicsDevice.Viewport.Height * 0.3f);
                _spriteBatch.DrawString(_scoreFont, _titleText, titlePosition, Color.White);

                // Draw start prompt below title
                Vector2 promptSize = _scoreFont.MeasureString(_startPrompt);
                Vector2 promptPosition = new Vector2(
                    (GraphicsDevice.Viewport.Width - promptSize.X) / 2,
                    titlePosition.Y + titleSize.Y + 50);
                _spriteBatch.DrawString(_scoreFont, _startPrompt, promptPosition, Color.White);

                _spriteBatch.End();
                break;

            case GameState.Playing:
                // Draw background layers without camera transform
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                foreach (var layer in _backgroundLayers)
                {
                    layer.Draw(_spriteBatch);
                }
                _spriteBatch.End();

                // Draw game elements with camera transform
                _spriteBatch.Begin(transformMatrix: _camera);
                foreach (var platform in _platforms)
                {
                    platform.Draw(_spriteBatch);
                }
                _player.Draw(_spriteBatch);
                _spriteBatch.End();

                // Draw score without camera transform
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_scoreFont, $"Score: {_score}", new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(_scoreFont, $"High Score: {_highScore}", new Vector2(10, 40), Color.White);
                _spriteBatch.End();
                break;

            case GameState.GameOver:
                _spriteBatch.Begin();

                // Draw "GAME OVER" text
                Vector2 gameOverSize = _scoreFont.MeasureString(_gameOverText);
                Vector2 gameOverPos = new Vector2(
                    (GraphicsDevice.Viewport.Width - gameOverSize.X) / 2,
                    GraphicsDevice.Viewport.Height * 0.3f);
                _spriteBatch.DrawString(_scoreFont, _gameOverText, gameOverPos, Color.Red);

                // Draw final score
                string finalScore = $"{_finalScoreText}{_score}";
                Vector2 scoreSize = _scoreFont.MeasureString(finalScore);
                Vector2 scorePos = new Vector2(
                    (GraphicsDevice.Viewport.Width - scoreSize.X) / 2,
                    gameOverPos.Y + gameOverSize.Y + 20);
                _spriteBatch.DrawString(_scoreFont, finalScore, scorePos, Color.White);

                // Draw high score
                string highScore = $"{_highScoreText}{_highScore}";
                Vector2 highScoreSize = _scoreFont.MeasureString(highScore);
                Vector2 highScorePos = new Vector2(
                    (GraphicsDevice.Viewport.Width - highScoreSize.X) / 2,
                    scorePos.Y + scoreSize.Y + 20);
                _spriteBatch.DrawString(_scoreFont, highScore, highScorePos, Color.Yellow);

                // Draw restart prompt
                Vector2 restartSize = _scoreFont.MeasureString(_restartPrompt);
                Vector2 restartPos = new Vector2(
                    (GraphicsDevice.Viewport.Width - restartSize.X) / 2,
                    highScorePos.Y + highScoreSize.Y + 40);
                _spriteBatch.DrawString(_scoreFont, _restartPrompt, restartPos, Color.White);

                // Draw quit prompt
                Vector2 quitSize = _scoreFont.MeasureString(_quitPrompt);
                Vector2 quitPos = new Vector2(
                    (GraphicsDevice.Viewport.Width - quitSize.X) / 2,
                    restartPos.Y + restartSize.Y + 20);
                _spriteBatch.DrawString(_scoreFont, _quitPrompt, quitPos, Color.White);

                _spriteBatch.End();
                break;
        }

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        _playerBodyTexture.Dispose();
        _playerArmTexture.Dispose();
        _playerLegTexture.Dispose();
        _platformTexture.Dispose();
        _skyTexture.Dispose();
        _mountainsTexture.Dispose();
        _cloudsTexture.Dispose();
        _foodTexture.Dispose();
        _orangeTexture.Dispose();
        _bananaTexture.Dispose();
        base.UnloadContent();
    }
}
