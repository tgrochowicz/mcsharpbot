using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.bots
{
    public class MCBotBase
    {
        public void Start(string username, string password, string servername)
        {
            //Create server
            //log in
            BeginAction();
        }

        public void Pause()
        {
            PauseAction();
        }        
        public void Stop()
        {
            StopAction();
            //log off
        }        


        private virtual void BeginAction()
        {        }
        private virtual void PauseAction()
        {        }
        private virtual void StopAction()
        {        }

        public bool DummyProperty
        {
            get;
            set;
        }

    }
}
