using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Networking;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.Entities
{
    abstract class IEntity
    {
        private bool _delete = false;

        protected bool canCollide = false;
        protected bool canBeHitByBullet = false;

        public abstract void OnUpdate(float deltatime, EntityManager entityManager, RenderWindow window);
        public abstract void OnFixedUpdate(float fixedDeltatime, EntityManager entityManager, RenderWindow window);
        public abstract void OnDraw(float deltatime, RenderWindow window);
        public abstract void OnTick(ConnectionBuffer buffer);



        public abstract Vector2f getPosition();
        public abstract Vector2f getSize();

        public void MarkForDeletion()
        {
            _delete = true;
        }

        public bool HasDeletionMark()
        {
            return _delete;
        }
    }
}
