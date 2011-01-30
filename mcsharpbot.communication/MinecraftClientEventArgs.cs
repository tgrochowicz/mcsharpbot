using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication
{
    public class MinecraftClientEventArgs : EventArgs
    {
        public MinecraftClientEventArgs() : base() { }
    }

    public class MinecraftClientConnectEventArgs : EventArgs
    {
        public MinecraftClientConnectEventArgs() : base() { }
    }

    public class MinecraftClientChatEventArgs : EventArgs
    {
        public string User, Message;

        public MinecraftClientChatEventArgs(string User, string Message)
            : base()
        {
            this.User = User;
            this.Message = Message;
        }
    }

    public class MinecraftClientLocationEventArgs : EventArgs
    {
        public Location PlayerLocation;

        public MinecraftClientLocationEventArgs(Location PlayerLocation)
            : base()
        {
            this.PlayerLocation = PlayerLocation;
        }
    }
}