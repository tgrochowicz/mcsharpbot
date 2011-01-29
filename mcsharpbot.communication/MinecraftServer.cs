using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace mcsharpbot.communication
{
    public class MinecraftServer
    {

        private Socket Socket;

        public IPEndPoint ServerAddress;
        public string ServerName, ServerMOTD, Password, Hash;
        public long MapSeed, Time;

        public MinecraftServer(Socket socket)
        {
            this.Socket = socket;
            this.ServerAddress = (IPEndPoint)this.Socket.RemoteEndPoint;
            this.ServerName = this.ServerMOTD = this.Password = this.Hash = "";
            this.MapSeed = this.Time = 0L;
        }
    }
}