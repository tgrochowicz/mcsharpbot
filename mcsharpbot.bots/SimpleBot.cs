using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.bots
{
    public class SimpleBot : MCBotBase
    {
        public override void  BeginAction()
        {
            Connection.ChatMessageReceived += new communication.MCServerConnection.MinecraftClientChatEventHandler(Connection_ChatMessageReceived);
 	        base.BeginAction();
        }

        void Connection_ChatMessageReceived(object sender, communication.MinecraftClientChatEventArgs args)
        {
            Console.WriteLine(args.User + ": " + args.Message);
        }

        public override void PauseAction()
        {
            base.PauseAction();
        }

        public override void StopAction()
        {
            base.StopAction();
        }

    }
}
