using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Login : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Login; }
        }

        public int Version;
        public string Username;
        public string ServerPassword;
        public long MapSeed;
        public byte Dimension;

        public void Read(NetworkStream stream)
        {
            this.Version = StreamHelper.ReadInt(stream); //entity id
            this.Username = StreamHelper.ReadString(stream); //server name
            this.ServerPassword = StreamHelper.ReadString(stream); //motd
            this.MapSeed = StreamHelper.ReadLong(stream); //map seed
            this.Dimension = (byte)stream.ReadByte(); //dimension
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);
            StreamHelper.WriteInt(stream, this.Version); //version
            StreamHelper.WriteString(stream, this.Username); //username
            StreamHelper.WriteString(stream, this.ServerPassword); //server password
            StreamHelper.WriteLong(stream, this.MapSeed); //map seed
            stream.WriteByte(this.Dimension); //not used

            stream.Flush();
        }
    }
}
