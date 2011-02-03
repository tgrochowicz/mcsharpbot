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
                    Connection.PlayerRotation.Yaw = 150;
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
