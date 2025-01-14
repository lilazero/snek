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
    private bool _gameOver;
    private float _moveTimer;
    private const float MoveInterval = 0.15f;

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
        ResetGame();
        base.Initialize();
    }

    private void ResetGame()
    {
        _snake = new Snake(new Point(10, 7));
        _food = new Food();
        _scoreCounter = new ScoreCounter();
        _food.Spawn(_grid, _snake.Body.ToList());
        _gameOver = false;
        _moveTimer = 0;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("GameFont");
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        if (_gameOver)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.R))
                ResetGame();
            return;
        }

        HandleInput();
        UpdateSnake(gameTime);

        base.Update(gameTime);
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
                _gameOver = true;
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

        // Draw grid cells
        foreach (var position in _snake.Body)
        {
            _spriteBatch.Draw(
                pixel,
                new Rectangle(
                    position.X * _grid.CellSize,
                    position.Y * _grid.CellSize,
                    _grid.CellSize - 1,
                    _grid.CellSize - 1
                ),
                Color.Green
            );
        }

        // Draw food
        _spriteBatch.Draw(
            pixel,
            new Rectangle(
                _food.Position.X * _grid.CellSize,
                _food.Position.Y * _grid.CellSize,
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

        if (_gameOver)
        {
            var text = "Game Over! Press R to restart";
            var textSize = _font.MeasureString(text);
            _spriteBatch.DrawString(
                _font,
                text,
                new Vector2(400 - textSize.X / 2, 300 - textSize.Y / 2),
                Color.White
            );
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
