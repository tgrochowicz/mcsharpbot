using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using mcsharpbot.communication.Entities;

namespace mcsharpbot.communication
{
    public class MinecraftServer
    {

        private Socket Socket;

        public IPEndPoint ServerAddress;
        public string ServerName, ServerMOTD, Password, Hash;
        public long MapSeed, Time;
        public EntityCollection Entities;

        public MinecraftServer(Socket socket)
        {
            this.Socket = socket;
            this.ServerAddress = (IPEndPoint)this.Socket.RemoteEndPoint;
            this.ServerName = this.ServerMOTD = this.Password = this.Hash = "";
            this.MapSeed = this.Time = 0L;
            Entities = new EntityCollection();
        }
        public string GetFriendlyTime()
        {
            long mod = Time % 24000;
            mod = mod / 1000;
            mod += 8;
            mod = mod % 24;
            string time = mod.ToString();
            mod = Time % 1000;
            double mins = mod / 1000.0 * 60.0;
            time = time + ":" + ((int)mins).ToString().PadLeft(2, '0') ;
            return time;
        }
    }
}