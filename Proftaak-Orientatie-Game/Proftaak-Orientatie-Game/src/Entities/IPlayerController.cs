using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace Proftaak_Orientatie_Game.Entities
{
    abstract class IPlayerController
    {
        public Vector2f Position { get; set; }
        public Vector2f Velocity { get; set; }

        public abstract void FixedUpdate(float fixedDeltatime);
        public abstract void Update(float deltatime);
    }
}
