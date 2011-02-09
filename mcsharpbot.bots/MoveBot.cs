using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.bots
{
    public class MoveBot : MCBotBase
    {
        public override void BeginAction()
        {
            Connection.ChatMessageReceived += new communication.MCServerConnection.MinecraftClientChatEventHandler(Connection_ChatMessageReceived);
            base.BeginAction();
        }

        void Connection_ChatMessageReceived(object sender, communication.MinecraftClientChatEventArgs args)
        {
            if (!args.User.Equals(Connection.Username))
            {
                if (args.Message.Trim() == "move")
                {
                    Connection.PlayerLocation.X = Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().X;
                    Connection.PlayerLocation.Y = Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().Y;
                    Connection.PlayerLocation.Z = Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().Z;
                }
                if (args.Message.Trim() == "movef")
                {
                    Connection.PlayerLocation.X = Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().ServerX;
                    Connection.PlayerLocation.Y = Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().ServerY;
                    Connection.PlayerLocation.Z = Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().ServerZ;
                }
            }
        }

        public override void PauseAction()
        {
            base.PauseAction();
        }

        public override void StopAction()
        {
            base.StopAction();
        }


        public override string BotName()
        {
            return "MoveBot";
        }


    }
}
