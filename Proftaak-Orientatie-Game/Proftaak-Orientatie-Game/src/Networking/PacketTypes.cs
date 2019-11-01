using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Proftaak_Orientatie_Game.Entities.Player;
using SFML.System;

namespace Proftaak_Orientatie_Game.Networking
{
    enum PACKET_TYPES : byte
    {
        PLAYER_SPAWN = 0,
        PLAYER_UPDATE = 1,
        PLAYER_SHOOT = 2,
        PLAYER_DISCONNECT = 3,
    }

    class Packet
    {
        public static PACKET_TYPES GetType(byte[] data)
        {
            return (PACKET_TYPES)data[0];
        }

        public static byte[] Serialize(IPacket obj)
        {
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public static T Deserialize<T>(byte[] data) where T : struct, IPacket
        {
            T obj = new T();
            int size = Marshal.SizeOf(obj);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(data, 0, ptr, size);

            obj = (T)Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);

            return obj;
        }
    }

    interface IPacket
    {
        PACKET_TYPES packetType { get; set; }
    }

    struct PlayerSpawnPacket : IPacket
    {
        public PACKET_TYPES packetType { get; set; }
        public int id;
        public Vector2f position;

        public PlayerSpawnPacket(int id, Vector2f pos)
        {
            packetType = PACKET_TYPES.PLAYER_SPAWN;

            this.id = id;
            position = pos;
        }
    }

    struct PlayerUpdatePacket : IPacket
    {
        public PACKET_TYPES packetType { get; set; }
        public int id;
        public float health;
        public Vector2f position;
        public Vector2f velocity;
        public Vector2f direction;

        public PlayerUpdatePacket(int id, float hp, Vector2f pos, Vector2f vel, Vector2f dir)
        {
            packetType = PACKET_TYPES.PLAYER_UPDATE;

            this.id = id;
            health = hp;
            position = pos;
            velocity = vel;
            direction = dir;
        }
    }

    struct PlayerDisconnectPacket : IPacket
    {
        public PACKET_TYPES packetType { get; set; }
        public int id;

        public PlayerDisconnectPacket(int id)
        {
            packetType = PACKET_TYPES.PLAYER_DISCONNECT;
            this.id = id;
        }
    }

    struct PlayerShootPacket : IPacket
    {
        public PACKET_TYPES packetType { get; set; }
        public int id;
        public Vector2f origin;
        public Vector2f direction;

        public PlayerShootPacket(int id, Vector2f origin, Vector2f direction)
        {
            packetType = PACKET_TYPES.PLAYER_SHOOT;

            this.id = id;
            this.origin = origin;
            this.direction = direction;
        }
    }
}
