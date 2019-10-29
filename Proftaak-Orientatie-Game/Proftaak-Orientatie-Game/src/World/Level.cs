using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proftaak_Orientatie_Game.World
{
    class Level
    {
        public Level() { }
        public Level(string filename)
        {

        }

        public virtual void OnDraw(float deltatime, RenderWindow window)
        {}
    }
}
