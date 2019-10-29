using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.SpriteUtils;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities
{
    class Player : IEntity
    {
        private const float TOTAL_SHOOT_COOLDOWN = 0.4f;
        private const float REPRESS_COOLDOWN_REDUCTION = 0.1f;
        private float _shootCooldown;
        private bool _isShootPressed;

        private readonly Sprite _sprite;
        private readonly IPlayerController _playerController;

        enum Direction { DOWN = 0, RIGHT, LEFT, UP }
        private readonly Animation[] _animations = {
            new Animation(16, 16, 32, new [] {0,1}),
            new Animation(16, 16, 32, new [] {2,3}),
            new Animation(16, 16, 32, new [] {4,5}),
            new Animation(16, 16, 32, new [] {6,7})
        };

        private Direction _currentDirection;

        public Player(Vector2f spawnPositon, IPlayerController playerController)
        {
            canBeHitByBullet = true;

            _playerController = playerController;

            Texture tex = new Texture("res/textures/player.png");
            _sprite = new Sprite(tex) {
                Position = spawnPositon,
                TextureRect = _animations[(int)Direction.DOWN].GetShape(),
                Scale = new Vector2f(3.0f, 3.0f),
                Origin = new Vector2f(_animations[0].GetShape().Width * 0.5f, _animations[0].GetShape().Height * 0.5f)
            };
            _currentDirection = Direction.DOWN;
        }

        public override void OnUpdate(float deltatime, EntityManager entityManager, RenderWindow window)
        {

            Direction previousDirection = _currentDirection;

            // Update the player controller
            _playerController.Position = _sprite.Position;
            _playerController.Update(window, deltatime);
            _sprite.Position = _playerController.Position;

            // Update the direction
            float angle = (float)-Math.Atan2(_playerController.Direction.Y, _playerController.Direction.X);
            while (angle < 0.0f)
                angle += 2.0f * (float)Math.PI;

            while(angle > 2.0f * (float)Math.PI)
                angle -= 2.0f * (float)Math.PI;

            _currentDirection = Direction.RIGHT;
            if (angle > Math.PI * 0.25f)
                _currentDirection = Direction.UP;
            if (angle > Math.PI * 0.75f)
                _currentDirection = Direction.LEFT;
            if (angle > Math.PI * 1.25f)
                _currentDirection = Direction.DOWN;
            if (angle > Math.PI * 1.75f)
                _currentDirection = Direction.RIGHT;

            // Shoot a bullet
            float speed = (float)Math.Sqrt(_playerController.Velocity.X * _playerController.Velocity.X +
                                           _playerController.Velocity.Y * _playerController.Velocity.Y);

            bool shoot = Keyboard.IsKeyPressed(Keyboard.Key.Space);

            _shootCooldown -= deltatime;
            if (_isShootPressed && !shoot)
                _shootCooldown -= REPRESS_COOLDOWN_REDUCTION;

            if (shoot && _shootCooldown <= 0.0f)
            {
                entityManager.ShootBullet(new Bullet(_sprite.Position, _playerController.Direction), 800.0f);
                _shootCooldown = TOTAL_SHOOT_COOLDOWN;
            }

            Console.WriteLine(_shootCooldown);

            _isShootPressed = shoot;

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
                _animations[(int) _currentDirection].Update(deltatime * speed * 2.0f);
            }

            _sprite.TextureRect = _animations[(int)_currentDirection].GetShape();
        }

        public override void OnFixedUpdate(float fixedDeltatime, EntityManager entityManager, RenderWindow window)
        {
            // Update the player controller
            _playerController.Position = _sprite.Position;
            _playerController.FixedUpdate(window, fixedDeltatime);
            _sprite.Position = _playerController.Position;
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.Draw(_sprite);
        }

        public override Vector2f getPosition()
        {
            return _sprite.Position;
        }

        public override Vector2f getSize()
        {
            return new Vector2f(_sprite.GetLocalBounds().Width, _sprite.GetLocalBounds().Height);
        }
    }
}
