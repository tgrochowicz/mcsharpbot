using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mcsharpbot.communication;
using System.Net;

namespace mcsharpbot.bots
{
    public class MCBotBase
    {
        public void Start(string username, string password, string servername, int port)
        {
            //Create server
            System.Net.IPAddress addr;
            IPAddress[] addrs = new IPAddress[0];
            try
            {
                addrs = Dns.GetHostAddresses(servername);
            }
            catch{

            }
            if (addrs != null && addrs.Length > 0)
            {
                addr = addrs[0];
                username = System.Web.HttpUtility.UrlEncode(username);
                password = System.Web.HttpUtility.UrlEncode(password);
                _connection = new MCServerConnection(username, password, new System.Net.IPEndPoint(addr, port));
                _connection.UseAuthentication = true;
                //log in
                _connection.Connect();
                BeginAction();
            }
            else
                throw new MinecraftClientGeneralException("Unable to connect to server");
        }
        private MCServerConnection _connection;
        public MCServerConnection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }

        }
        public void Pause()
        {
            PauseAction();
        }        
        public void Stop()
        {
            StopAction();
            //log off
            _connection = null;
        }        


        public virtual void BeginAction()
        {        }
        public virtual void PauseAction()
        {        }
        public virtual void StopAction()
        {        }


    }
}
