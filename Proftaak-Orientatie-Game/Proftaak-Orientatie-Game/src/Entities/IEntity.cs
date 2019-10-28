using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace Proftaak_Orientatie_Game.Entities
{
    abstract class IEntity
    {
        private bool _delete = false;

        public abstract void OnUpdate(float deltatime);
        public abstract void OnFixedUpdate(float fixedDeltatime);
        public abstract void OnDraw(float deltatime, RenderWindow window);

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
