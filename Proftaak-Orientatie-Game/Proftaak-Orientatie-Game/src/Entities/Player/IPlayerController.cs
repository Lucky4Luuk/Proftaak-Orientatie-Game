using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.Entities.Player
{
    abstract class IPlayerController
    {
        public Vector2f Position { get; set; }
        public Vector2f Velocity { get; set; }
        public Vector2f Direction { get; set; }

        public abstract void FixedUpdate(RenderWindow window, float fixedDeltatime);
        public abstract void Update(RenderWindow window, float deltatime);
    }
}
