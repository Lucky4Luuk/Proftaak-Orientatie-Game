using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.SpriteUtils;
using Proftaak_Orientatie_Game.World;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.Networking;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities.Player
{
    class Player : IEntity
    {
        public bool Active { get; set; } = false;

        public const float MAX_HEALTH = 100.0f;
        public float Health { get; set; } = MAX_HEALTH;

        private readonly Sprite _sprite;
        private readonly IPlayerController _playerController;

        private const float FLASH_TIME = 0.2f;
        private float _flash;

        public Camera Camera { get; set; }

        enum Direction { DOWN = 0, RIGHT, LEFT, UP }
        private readonly Animation[] _animations = {
            new Animation(16, 16, 64, new [] {6,7}),
            new Animation(16, 16, 64, new [] {2,3}),
            new Animation(16, 16, 64, new [] {4,5}),
            new Animation(16, 16, 64, new [] {0,1})
        };

        private Direction _currentDirection;

        private Texture[] _textures;

        public Player(Vector2f spawnPositon, IPlayerController playerController, Texture[] playerTexture, Texture healthBarTexture, EntityManager manager, Camera _camera)
        {
            _textures = playerTexture;

            manager.Add(new HealthBar(healthBarTexture, this));

            canBeHitByBullet = true;

            _playerController = playerController;

            _sprite = new Sprite {
                Position = spawnPositon,
                TextureRect = _animations[(int)Direction.DOWN].GetShape(),
                Origin = new Vector2f(_animations[0].GetShape().Width, _animations[0].GetShape().Height)
            };
            _currentDirection = Direction.DOWN;

            Camera = _camera;
        }

        public void SetId(int id)
        {
            _sprite.Texture = _textures[id % _textures.Length];
        }

        public override void OnUpdate(float deltatime, EntityManager entityManager, ConnectionBuffer buffer, RenderWindow window)
        {

            Direction previousDirection = _currentDirection;

            // Update the player controller
            _playerController.Position = _sprite.Position;
            _playerController.Update(window, deltatime, Camera);
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

            float speed = (float)Math.Sqrt(_playerController.Velocity.X * _playerController.Velocity.X +
                                           _playerController.Velocity.Y * _playerController.Velocity.Y);

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

            // Shoot a bullet
            if (_playerController.ShotOrigin.HasValue)
            {
                entityManager.ShootBullet(true, -1,
                    new Bullet.Bullet(_playerController.ShotOrigin.Value, _playerController.ShotDirection), 800.0f);

                buffer.Add(new PlayerShootPacket(-1, _playerController.ShotOrigin.Value, _playerController.ShotDirection));
            }

            _sprite.TextureRect = _animations[(int)_currentDirection].GetShape();

            if (_playerController.DeletionMark)
                MarkForDeletion();

            if (!Active)
            {
                if(_playerController.Health != Health)
                    Hit();

                Health = _playerController.Health;
            }
        }

        public override void OnFixedUpdate(float fixedDeltatime, EntityManager entityManager, ConnectionBuffer buffer, RenderWindow window)
        {
            // Update the player controller
            _playerController.Position = _sprite.Position;
            _playerController.FixedUpdate(window, fixedDeltatime);
            _sprite.Position = _playerController.Position;

            if(_playerController.DeletionMark)
                MarkForDeletion();
        }

        public void Hit()
        {
            _flash = 1.0f;
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            _flash -= deltatime / FLASH_TIME;
            if (_flash < 0.0f)
                _flash = 0.0f;

            _sprite.Color = new Color((byte)((1.0f - _flash) * 255), (byte)((1.0f - _flash) * 255), (byte)((1.0f - _flash) * 255));

            window.Draw(_sprite);
        }

        public override void OnTick(ConnectionBuffer buffer)
        {
            if(Active)
                buffer.Add(new PlayerUpdatePacket(-1, Health, _sprite.Position, _playerController.Velocity, _playerController.Direction));
        }

        public override Vector2f getPosition()
        {
            return _sprite.Position;
        }

        public override Vector2f getSize()
        {
            return new Vector2f(_sprite.GetLocalBounds().Width, _sprite.GetLocalBounds().Height);
        }

        public void setPosition(Vector2f newPos)
        {
            _sprite.Position = newPos;
            //_playerController.Position = newPos;
        }

        public int GetId()
        {
            return ((NetworkController) _playerController).Id;
        }
    }
}
