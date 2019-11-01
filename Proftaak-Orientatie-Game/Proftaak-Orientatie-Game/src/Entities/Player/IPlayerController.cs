using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.Entities.Player
{
    abstract class IPlayerController
    {
        public bool DeletionMark { get; set; } = false;
        public Vector2f Position { get; set; }
        public Vector2f Velocity { get; set; }
        public Vector2f Direction { get; set; }

        public Vector2f? ShotOrigin { get; set; } = null;
        public Vector2f ShotDirection { get; set; }

        public float Health { get; set; } = Player.MAX_HEALTH;

        public abstract void FixedUpdate(RenderWindow window, float fixedDeltatime);
        public abstract void Update(RenderWindow window, float deltatime, Camera camera);
    }
}
