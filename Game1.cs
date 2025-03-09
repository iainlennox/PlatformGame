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
    private Texture2D _playerTexture;
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
    private string _titleText = "PLATFORM RUNNER";
    private string _startPrompt = "Press SPACE to Start";
    private int _highScore;
    private string _gameOverText = "GAME OVER";
    private string _finalScoreText = "Final Score: ";
    private string _highScoreText = "High Score: ";
    private string _restartPrompt = "Press SPACE to Play Again";
    private string _quitPrompt = "Press ESC to Quit";

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _platforms = new List<Platform>();
        _random = new Random();
        _score = 0;
        _highScore = 0;
        _timeSinceStart = 0;
        _currentGameState = GameState.TitleScreen;
    }

    protected override void Initialize()
    {
        // Create textures
        _playerTexture = new Texture2D(GraphicsDevice, 32, 32);
        Color[] playerColorData = new Color[32 * 32];
        for (int i = 0; i < playerColorData.Length; i++)
            playerColorData[i] = Color.Red;
        _playerTexture.SetData(playerColorData);

        _platformTexture = new Texture2D(GraphicsDevice, 1, 1);
        Color[] platformColorData = new Color[1];
        platformColorData[0] = Color.Green;
        _platformTexture.SetData(platformColorData);

        // Create the player
        _player = new Player(_playerTexture, new Vector2(100, 100));

        // Create initial platforms
        _lastPlatformX = 50;
        GenerateInitialPlatforms();

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

    private void GenerateNextPlatform()
    {
        float gap = _random.NextSingle() * (MAX_PLATFORM_GAP - MIN_PLATFORM_GAP) + MIN_PLATFORM_GAP;
        float platformX = _lastPlatformX + gap;
        float platformY = _baseY + (_random.NextSingle() * 2 - 1) * PLATFORM_Y_VARIATION;
        int platformWidth = _random.Next(100, 200);
        
        _platforms.Add(new Platform(_platformTexture, 
            new Rectangle((int)platformX, (int)platformY, platformWidth, 20)));
        _lastPlatformX = platformX + platformWidth;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
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
        _player = new Player(_playerTexture, new Vector2(100, 100));
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
        _playerTexture.Dispose();
        _platformTexture.Dispose();
        base.UnloadContent();
    }
}
