using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SFML.System;

namespace Proftaak_Orientatie_Game.Networking
{
    enum PACKET_TYPES
    {
        GENERAL = -1,
        SET_CLIENT_ID = 0,
        CLIENT_POSITION = 1,
        ENTITY_POSITION = 2,
    }

    class Packet
    {
        public static byte[] Serialize(IPacket obj)
        {
            int size = Marshal.SizeOf(obj);
            Console.WriteLine(size);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public static T Deserialize<T>(byte[] data) where T : struct, IPacket
        {
            T obj = new T { };
            obj.packetType = PACKET_TYPES.GENERAL;
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

    struct ClientIDPacket : IPacket
    {
        public PACKET_TYPES packetType { get; set; }
    }

    struct PositionLookPacket : IPacket
    {
        public PACKET_TYPES packetType { get; set; }

        //Packet specific fields
        public uint positionX; //float x * 10 then floored
        public uint positionY; //float y * 10 then floored
        public UInt16 rotation;
    }
}
