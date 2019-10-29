using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.src.SpriteUtils;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities
{
    class Player : IEntity
    {
        private readonly Sprite _sprite;
        private readonly IPlayerController _playerController;

        enum Direction { DOWN = 0, RIGHT, LEFT, UP }
        private readonly Animation[] _animations = new Animation[]
        {
            new Animation(16, 16, 32, new [] {0,1}),
            new Animation(16, 16, 32, new [] {2,3}),
            new Animation(16, 16, 32, new [] {4,5}),
            new Animation(16, 16, 32, new [] {6,7})
        };

        private Direction _currentDirection;

        public Player(Vector2f spawnPositon, IPlayerController playerController)
        {
            _playerController = playerController;

            Texture tex = new Texture("res/textures/player.png");
            _sprite = new Sprite(tex) {
                Position = spawnPositon,
                TextureRect = _animations[(int)Direction.DOWN].GetShape(),
                Scale = new Vector2f(3.0f, 3.0f)
            };
            _currentDirection = Direction.DOWN;
        }

        public override void OnUpdate(float deltatime)
        {
            Direction previousDirection = _currentDirection;

            // Update the player controller
            _playerController.Position = _sprite.Position;
            _playerController.Update(deltatime);
            _sprite.Position = _playerController.Position;

            // Update the direction
            if (_playerController.Velocity.X > 0.0f)
                _currentDirection = Direction.RIGHT;
            else if(_playerController.Velocity.X < 0.0f)
                _currentDirection = Direction.LEFT;

            if (_playerController.Velocity.Y > 0.0f)
                _currentDirection = Direction.DOWN;
            else if (_playerController.Velocity.Y < 0.0f)
                _currentDirection = Direction.UP;

            // Update animation
            if (_playerController.Velocity.X == 0.0f && _playerController.Velocity.Y == 0.0f)
            {
                _animations[(int) _currentDirection].SetFrame(0.0f);
            }
            else
            {
                if (_currentDirection != previousDirection)
                {
                    _animations[(int) _currentDirection].SetFrame(0.0f);
                }

                float speed = (float) Math.Sqrt(_playerController.Velocity.X * _playerController.Velocity.X +
                                                _playerController.Velocity.Y * _playerController.Velocity.Y);

                _animations[(int) _currentDirection].Update(deltatime * speed * 2.0f);
            }

            _sprite.TextureRect = _animations[(int)_currentDirection].GetShape();
        }

        public override void OnFixedUpdate(float fixedDeltatime)
        {
            // Update the player controller
            _playerController.Position = _sprite.Position;
            _playerController.FixedUpdate(fixedDeltatime);
            _sprite.Position = _playerController.Position;
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.Draw(_sprite);
        }
    }
}
