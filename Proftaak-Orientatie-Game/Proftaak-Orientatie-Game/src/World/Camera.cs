using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proftaak_Orientatie_Game.World
{
    class Camera
    {
        public View viewport = new View(new Vector2f(0, 0), new Vector2f(256, 256));

        public Camera()
        {}
    }
}
