using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Networking;
using Proftaak_Orientatie_Game.World;
using SFML.Graphics;

namespace Proftaak_Orientatie_Game.Entities.Player
{
    class NetworkController : IPlayerController
    {
        private readonly SceneBuffer _buffer;
        public int Id { get; }

        public NetworkController(SceneBuffer buffer, int id)
        {
            _buffer = buffer;
            Id = id;
        }

        public override void FixedUpdate(RenderWindow window, float fixedDeltatime)
        {
            PlayerUpdatePacket? packet = _buffer.GetData(Id);

            if (packet.HasValue)
            {
                Position = packet.Value.position;
                Velocity = packet.Value.velocity;
                Direction = packet.Value.direction;
            }
            else
            {
                DeletionMark = true;
            }
        }

        public override void Update(RenderWindow window, float deltatime, Camera camera)
        {
            PlayerUpdatePacket? packet = _buffer.GetData(Id);

            if (packet.HasValue)
            {
                Position = packet.Value.position;
                Velocity = packet.Value.velocity;
                Direction = packet.Value.direction;
                Health = packet.Value.health;
            }
            else
            {
                DeletionMark = true;
            }
        }
    }
}
