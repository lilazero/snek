using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace snek;

public class Snek : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GameGrid _grid;
    private Snake _snake;
    private Food _food;
    private ScoreCounter _scoreCounter;
    private SpriteFont _font;
    private GameState _gameState;
    private float _moveTimer;
    private const float MoveInterval = 0.15f;
    private KeyboardState _previousKeyboardState;

    public Snek()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
    }

    protected override void Initialize()
    {
        _grid = new GameGrid(20, 15, 30);
        _gameState = GameState.MainMenu;
        _previousKeyboardState = Keyboard.GetState();
        base.Initialize();
    }

    private void ResetGame()
    {
        _snake = new Snake(new Point(10, 7));
        _food = new Food();
        _scoreCounter = new ScoreCounter();
        _food.Spawn(_grid, _snake.Body.ToList());
        _moveTimer = 0;
        _gameState = GameState.Playing;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("GameFont");
    }

    protected override void Update(GameTime gameTime)
    {
        var currentKeyboardState = Keyboard.GetState();

        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || currentKeyboardState.IsKeyDown(Keys.Escape)
        )
            Exit();

        switch (_gameState)
        {
            case GameState.MainMenu:
                if (IsKeyPressed(Keys.Enter, currentKeyboardState))
                {
                    ResetGame();
                }
                break;

            case GameState.Playing:
                HandleInput();
                UpdateSnake(gameTime);
                break;

            case GameState.GameOver:
                if (IsKeyPressed(Keys.R, currentKeyboardState))
                {
                    ResetGame();
                }
                else if (IsKeyPressed(Keys.M, currentKeyboardState))
                {
                    _gameState = GameState.MainMenu;
                }
                break;
        }

        _previousKeyboardState = currentKeyboardState;
        base.Update(gameTime);
    }

    private bool IsKeyPressed(Keys key, KeyboardState currentKeyboardState)
    {
        return currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }

    private void HandleInput()
    {
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.Left))
            _snake.SetDirection(new Point(-1, 0));
        if (keyState.IsKeyDown(Keys.Right))
            _snake.SetDirection(new Point(1, 0));
        if (keyState.IsKeyDown(Keys.Up))
            _snake.SetDirection(new Point(0, -1));
        if (keyState.IsKeyDown(Keys.Down))
            _snake.SetDirection(new Point(0, 1));
    }

    private void UpdateSnake(GameTime gameTime)
    {
        _moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_moveTimer >= MoveInterval)
        {
            _moveTimer = 0;
            _snake.Move();

            if (_grid.IsOutOfBounds(_snake.Head) || _snake.CollidesWith(_snake.Head))
            {
                _gameState = GameState.GameOver;
                return;
            }

            if (_snake.Head == _food.Position)
            {
                _snake.Grow();
                _scoreCounter.Increment();
                _food.Spawn(_grid, _snake.Body.ToList());
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        switch (_gameState)
        {
            case GameState.MainMenu:
                var titleText = "SNAKE GAME";
                var startText = "Press ENTER to Start";
                var titleSize = _font.MeasureString(titleText);
                var startSize = _font.MeasureString(startText);

                _spriteBatch.DrawString(
                    _font,
                    titleText,
                    new Vector2(400 - titleSize.X / 2, 250),
                    Color.Green
                );
                _spriteBatch.DrawString(
                    _font,
                    startText,
                    new Vector2(400 - startSize.X / 2, 300),
                    Color.White
                );
                break;

            case GameState.Playing:
                // Draw walls with offset
                for (int x = 0; x <= _grid.Width + 1; x++)
                {
                    // Draw top wall (moved down by 1)
                    _spriteBatch.Draw(
                        pixel,
                        new Rectangle(
                            x * _grid.CellSize,
                            0,
                            _grid.CellSize - 1,
                            _grid.CellSize - 1
                        ),
                        Color.DarkGray
                    );

                    // Draw bottom wall
                    _spriteBatch.Draw(
                        pixel,
                        new Rectangle(
                            x * _grid.CellSize,
                            (_grid.Height + 1) * _grid.CellSize,
                            _grid.CellSize - 1,
                            _grid.CellSize - 1
                        ),
                        Color.DarkGray
                    );
                }

                for (int y = 0; y <= _grid.Height + 1; y++)
                {
                    // Draw left wall (moved right by 1)
                    _spriteBatch.Draw(
                        pixel,
                        new Rectangle(
                            0,
                            y * _grid.CellSize,
                            _grid.CellSize - 1,
                            _grid.CellSize - 1
                        ),
                        Color.DarkGray
                    );

                    // Draw right wall
                    _spriteBatch.Draw(
                        pixel,
                        new Rectangle(
                            (_grid.Width + 1) * _grid.CellSize,
                            y * _grid.CellSize,
                            _grid.CellSize - 1,
                            _grid.CellSize - 1
                        ),
                        Color.DarkGray
                    );
                }

                // Adjust game elements to account for the offset
                // Draw grid cells
                foreach (var position in _snake.Body)
                {
                    _spriteBatch.Draw(
                        pixel,
                        new Rectangle(
                            (position.X + 1) * _grid.CellSize,
                            (position.Y + 1) * _grid.CellSize,
                            _grid.CellSize - 1,
                            _grid.CellSize - 1
                        ),
                        Color.Green
                    );
                }

                // Draw food with offset
                _spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        (_food.Position.X + 1) * _grid.CellSize,
                        (_food.Position.Y + 1) * _grid.CellSize,
                        _grid.CellSize - 1,
                        _grid.CellSize - 1
                    ),
                    Color.Red
                );

                // Draw score
                _spriteBatch.DrawString(
                    _font,
                    $"Score: {_scoreCounter.Score}",
                    new Vector2(700, 10),
                    Color.White
                );
                break;

            case GameState.GameOver:
                // ... existing game over drawing code ...
                var gameOverText = "Game Over!";
                var restartText = "Press R to Restart";
                var menuText = "Press M for Main Menu";
                var goSize = _font.MeasureString(gameOverText);

                _spriteBatch.DrawString(
                    _font,
                    gameOverText,
                    new Vector2(400 - goSize.X / 2, 250),
                    Color.Red
                );
                _spriteBatch.DrawString(
                    _font,
                    restartText,
                    new Vector2(400 - _font.MeasureString(restartText).X / 2, 300),
                    Color.White
                );
                _spriteBatch.DrawString(
                    _font,
                    menuText,
                    new Vector2(400 - _font.MeasureString(menuText).X / 2, 330),
                    Color.White
                );
                _spriteBatch.DrawString(
                    _font,
                    $"Final Score: {_scoreCounter.Score}",
                    new Vector2(
                        400 - _font.MeasureString($"Final Score: {_scoreCounter.Score}").X / 2,
                        370
                    ),
                    Color.White
                );
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private Texture2D pixel
    {
        get
        {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }
    }
}
