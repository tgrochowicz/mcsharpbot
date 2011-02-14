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
                    Connection.PlayerLocation.X = 56.06956617524753D;
                    Connection.PlayerLocation.Y = 65.0D;
                    Connection.PlayerLocation.Z = 23.757122609011454D;
                }
                if (args.Message.Trim() == "movef")
                {
                    Connection.PlayerLocation.X = Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().X;
                    Connection.PlayerLocation.Y = Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().Y;
                    Connection.PlayerLocation.Z = Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().Z;
                }
                if (args.Message.Trim() == "location")
                {
                    mcsharpbot.communication.Packets.Types.Chat c = new communication.Packets.Types.Chat()
                    {
                        Message = String.Format("X: {0}, Y: {1}, Z: {2}", Connection.PlayerLocation.X, Connection.PlayerLocation.Y, Connection.PlayerLocation.Z)
                    };
                    Connection.SendPacket(c);
                }
                if (args.Message.Trim() == "plocation")
                {
                    mcsharpbot.communication.Packets.Types.Chat c = new communication.Packets.Types.Chat()
                    {
                        Message = String.Format("X: {0}, Y: {1}, Z: {2}", Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().X,
                            Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().Y,
                            Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().Z)
                    };
                    Connection.SendPacket(c);
                }
                if (args.Message.Trim() == "plocations")
                {
                    mcsharpbot.communication.Packets.Types.Chat c = new communication.Packets.Types.Chat()
                    {
                        Message = String.Format("X: {0}, Y: {1}, Z: {2}", Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().ServerX,
                            Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().ServerY,
                            Connection.GetServer().Entities.Entities.Where(e => e is mcsharpbot.communication.Entities.NamedEntityType).First().ServerZ)
                    };
                    Connection.SendPacket(c);
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
