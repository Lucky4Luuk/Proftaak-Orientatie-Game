using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Networking;
using SFML.Graphics;

namespace Proftaak_Orientatie_Game.Entities.Player
{
    class NetworkController : IPlayerController
    {
        private readonly SceneBuffer _buffer;
        private readonly int _id;
        
        public NetworkController(SceneBuffer buffer, int id)
        {
            _buffer = buffer;
            _id = id;
        }

        public override void FixedUpdate(RenderWindow window, float fixedDeltatime)
        {
            Position = _buffer.GetData(_id).position;
            Velocity = _buffer.GetData(_id).velocity;
            Direction = _buffer.GetData(_id).direction;
        }

        public override void Update(RenderWindow window, float deltatime)
        {
            Position = _buffer.GetData(_id).position;
            Velocity = _buffer.GetData(_id).velocity;
            Direction = _buffer.GetData(_id).direction;
        }
    }
}
