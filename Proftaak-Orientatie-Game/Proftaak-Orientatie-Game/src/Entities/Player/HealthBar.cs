using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities.Player;
using Proftaak_Orientatie_Game.Networking;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.Entities.Player
{
    class HealthBar : IEntity
    {
        private static readonly Vector2f OFFSET = new Vector2f(0.0f, -40.0f);

        private readonly Player _target;

        private readonly Sprite _background;
        private readonly Sprite _bar;

        public HealthBar (Texture healthBarTexture, Player target)
        {
            _target = target;

            _background = new Sprite(healthBarTexture)
            {
                TextureRect = new IntRect(0, 0, 16, 8),
                Scale = new Vector2f(3.0f, 3.0f),
                Origin = new Vector2f(8.0f, 4.0f)
            };

            _bar = new Sprite(healthBarTexture)
            {
                Scale = new Vector2f(3.0f, 3.0f),
                Origin = new Vector2f(8.0f, 4.0f)
            };
        }

        public override void OnUpdate(float deltatime, EntityManager entityManager, ConnectionBuffer buffer, RenderWindow window)
        {
            if(_target.HasDeletionMark())
                MarkForDeletion();
        }

        public override void OnFixedUpdate(float fixedDeltatime, EntityManager entityManager, ConnectionBuffer buffer, RenderWindow window)
        {}

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            _bar.TextureRect = new IntRect(0, 8, (int)(16 * _target.Health / Player.MAX_HEALTH), 8);
            _bar.Position = _target.getPosition() + OFFSET;
            _background.Position = _target.getPosition() + OFFSET;

            window.Draw(_background);
            window.Draw(_bar);
        }

        public override void OnTick(ConnectionBuffer buffer)
        {
        }

        public override Vector2f getPosition()
        {
            return _target.getPosition();
        }

        public override Vector2f getSize()
        {
            return _target.getSize();
        }
    }
}
