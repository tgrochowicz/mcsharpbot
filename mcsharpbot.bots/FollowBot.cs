using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.bots
{
    public class FollowBot : MCBotBase
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
                if (args.Message.Trim().Equals("mcb-time"))
                {
                    Connection.SendPacket(new mcsharpbot.communication.Packets.Types.Chat { Message = Connection.GetServer().GetFriendlyTime() });
                }
                else
                {
                    OnFeedbackReceived(this, new BotFeedbackEventArgs(args.User + ": " + args.Message));
                    Connection.SendPacket(new mcsharpbot.communication.Packets.Types.Chat { Message = args.User + " said " + args.Message });
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
            return "FollowBot";
        }

    }
}
