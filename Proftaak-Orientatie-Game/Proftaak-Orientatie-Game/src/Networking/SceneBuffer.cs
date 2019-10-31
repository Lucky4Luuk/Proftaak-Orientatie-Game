using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proftaak_Orientatie_Game.Entities;
using Proftaak_Orientatie_Game.Entities.Bullet;
using Proftaak_Orientatie_Game.Entities.Player;
using SFML.Graphics;
using SFML.System;

namespace Proftaak_Orientatie_Game.Networking
{
    class SceneBuffer
    {
        private readonly Texture _playerTexture;
        private readonly Texture _healthBarTexture;
        private readonly EntityManager _entityManager;

        private int _myId; 

        private readonly Dictionary<int, PlayerUpdatePacket> _players = new Dictionary<int, PlayerUpdatePacket>();

        public SceneBuffer(Texture playerTexture, Texture healthBarTexture, EntityManager manager)
        {
            _playerTexture = playerTexture;
            _healthBarTexture = healthBarTexture;
            _entityManager = manager;
        }

        public void Process(byte[] data)
        {
            if (Packet.GetType(data) == PACKET_TYPES.PLAYER_SPAWN)
            {
                _myId = Packet.Deserialize<PlayerSpawnPacket>(data).id;
            }

            if (Packet.GetType(data) == PACKET_TYPES.PLAYER_UPDATE)
            {
                PlayerUpdatePacket packet = Packet.Deserialize<PlayerUpdatePacket>(data);

                if (packet.id == _myId)
                    return;

                lock (_players)
                {
                    if (_players.ContainsKey(packet.id))
                    {
                        _players[packet.id] = packet;
                    }
                    else
                    {
                        _players.Add(packet.id, packet);

                        _entityManager.Add(new Player(packet.position, new NetworkController(this, packet.id), _playerTexture, _healthBarTexture, _entityManager, new World.Camera()));
                    }
                }
            }

            if (Packet.GetType(data) == PACKET_TYPES.PLAYER_DISCONNECT)
            {
                PlayerDisconnectPacket packet = Packet.Deserialize<PlayerDisconnectPacket>(data);

                lock(_players)
                    _players.Remove(packet.id);
            }

            if (Packet.GetType(data) == PACKET_TYPES.PLAYER_SHOOT)
            {
                PlayerShootPacket packet = Packet.Deserialize<PlayerShootPacket>(data);

                if(packet.id == _myId)
                    return;

                _entityManager.ShootBullet(new Bullet(packet.origin, packet.direction), 800.0f);
            }
        }

        public PlayerUpdatePacket? GetData(int id)
        {
            PlayerUpdatePacket packet;
            lock (_players)
            {
                if (!_players.ContainsKey(id))
                    return null;

                packet = _players[id];
            }

            return packet;
        }


    }
}
