using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication
{
    public class MinecraftClientGeneralException : Exception
    {
        public MinecraftClientGeneralException() : base("An unknown error occured.") { }
        public MinecraftClientGeneralException(Exception e) : base("An error occured.", e) { }
        public MinecraftClientGeneralException(String msg) : base("An error occured: " + msg) { }
    }

    public class MinecraftClientConnectException : Exception
    {
        public MinecraftClientConnectException() : base("Could not connect to specified server.") { }
        public MinecraftClientConnectException(Exception e) : base("Could not connect to specified server.", e) { }
        public MinecraftClientConnectException(string msg) : base("Could not connect to specified server: " + msg) { }
    }
}