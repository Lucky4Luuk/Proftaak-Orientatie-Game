using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Proftaak_Orientatie_Game.Entities
{
    class Player : IEntity
    {
        private readonly Sprite _sprite;
        private readonly IPlayerController _playerController;

        public Player(IPlayerController playerController)
        {
            _playerController = playerController;

            Texture tex = new Texture("res/textures/player.png");
            _sprite = new Sprite(tex)
            {
                Position = new Vector2f(200.0f, -250.0f)
            };
        }

        public override void OnUpdate(float deltatime)
        {
            _playerController.position = _sprite.Position;
            _playerController.Update(deltatime);
            _sprite.Position = _playerController.position;
        }

        public override void OnFixedUpdate(float fixedDeltatime)
        {
            _playerController.position = _sprite.Position;
            _playerController.FixedUpdate(fixedDeltatime);
            _sprite.Position = _playerController.position;
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.Draw(_sprite);
        }
    }
}
