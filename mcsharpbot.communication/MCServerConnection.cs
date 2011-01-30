using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace mcsharpbot.communication
{
    public class MCServerConnection : IDisposable
    {
        private string Username, Password;
        private Socket MainSocket;
        private IPEndPoint ServerAddress;
        private NetworkStream Stream;
        private MinecraftServer Server;

        public const int VERSION = 12;
        public bool UseAuthentication;
        public bool Connected
        {
            get
            {
                return _connected;
            }
            set
            {
                _connected = value;
                if (value == true)
                {
                    OnConnectedToServer(this, new MinecraftClientConnectEventArgs());
                }
                else
                {
                    OnDisconnectedFromServer(this, new MinecraftClientConnectEventArgs());
                }
            }
        }
        public Location PlayerLocation
        {
            get
            {
                return _playerLocation;
            }
            set
            {
                _playerLocation = value;
                OnPlayerLocationChanged(this, new MinecraftClientLocationEventArgs(value));
            }
        }
        public Rotation PlayerRotation
        {
            get
            {
                return _playerRotation;
            }
            set
            {
                _playerRotation = value;
                //OnPlayerRotationChanged(this, new MinecraftClientLocationEventArgs(value));
            }
        }

        private int EntityID;
        private string SessionID;
        private Location _playerLocation;
        private Rotation _playerRotation;
        private bool _connected, OnGround;

        public delegate void MinecraftClientConnectEventHandler(object sender, MinecraftClientConnectEventArgs args);
        public delegate void MinecraftClientChatEventHandler(object sender, MinecraftClientChatEventArgs args);
        public delegate void MinecraftClientLocationEventHandler(object sender, MinecraftClientLocationEventArgs args);
        public event MinecraftClientConnectEventHandler ConnectedToServer, DisconnectedFromServer;
        public event MinecraftClientChatEventHandler ChatMessageReceived;
        public event MinecraftClientLocationEventHandler PlayerLocationChanged;

        public Thread DataThread;
        private bool disposed = false;

        public MCServerConnection(string Username, string Password, IPEndPoint Address)
        {
            this.Username = Username;
            this.Password = Password;
            this.ServerAddress = Address;
            this.SessionID = "";
            this.Connected = false;
        }
        public void Connect()
        {
            if (UseAuthentication)
            {
                this.SessionID = Authenticate();
                if (this.SessionID == "")
                {
                    Debug.Severe(new MinecraftClientConnectException("Authentication Failed."));
                    return;
                }
            }

            this.MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            while (!MainSocket.Connected)
            {
                try
                {
                    this.MainSocket.Connect(ServerAddress);
                }
                catch (Exception e) { Debug.Severe(e); }
            }

            this.Stream = new NetworkStream(MainSocket);
            this.Server = new MinecraftServer(MainSocket);
            this.Server.Password = "Password";

            if (!SendInitialPackets())
            {
                Debug.Severe(new MinecraftClientConnectException());
                return;
            }

            this.Connected = true;
            this.PlayerLocation = new Location(0D, 0D, 0D, 0D + 1.65);
            this.PlayerRotation = new Rotation(0F, 0F);

            DataThread = new Thread(HandleData);
            DataThread.Start();
        }

        private void HandleData()
        {
            byte id;

            try
            {
                while (MainSocket.Connected && (int)(id = (byte)Stream.ReadByte()) != -1)
                {
                    try
                    {
                        switch (id)
                        {
                            case 0x00: //Ping
                                Stream.WriteByte(0x00);
                                break;
                            case 0x03: //Chat
                                try
                                {
                                    String s = StreamHelper.ReadString(Stream);
                                    if (s[0] != '<') break;
                                    String user = s.Substring(1, s.IndexOf(">") - 1);
                                    String msg = s.Replace("<" + user + "> ", "");
                                    OnChatMessageReceived(this, new MinecraftClientChatEventArgs(user, msg));
                                }
                                catch (IndexOutOfRangeException e) { }
                                break;
                            case 0x04:
                                this.Server.Time = StreamHelper.ReadLong(Stream);
                                break;
                            case 0x05: StreamHelper.ReadBytes(Stream, 10); break;
                            case 0x06: StreamHelper.ReadBytes(Stream, 12); break;
                            case 0x07: StreamHelper.ReadBytes(Stream, 9); break;
                            case 0x08: StreamHelper.ReadBytes(Stream, 2); break;
                            case 0x09: break;
                            case 0x0A: StreamHelper.ReadBytes(Stream, 1); break;
                            case 0x0B: StreamHelper.ReadBytes(Stream, 33); break;
                            case 0x0C: StreamHelper.ReadBytes(Stream, 9); break;
                            case 0x0D:
                                this.PlayerLocation.X = StreamHelper.ReadDouble(Stream);
                                this.PlayerLocation.Y = StreamHelper.ReadDouble(Stream);
                                this.PlayerLocation.Stance = StreamHelper.ReadDouble(Stream);
                                this.PlayerLocation.Z = StreamHelper.ReadDouble(Stream);
                                this.PlayerRotation.Pitch = StreamHelper.ReadFloat(Stream);
                                this.PlayerRotation.Yaw = StreamHelper.ReadFloat(Stream);
                                this.OnGround = StreamHelper.ReadBoolean(Stream);
                                OnPlayerLocationChanged(this, new MinecraftClientLocationEventArgs(this.PlayerLocation));
                                break;
                            case 0x0E: StreamHelper.ReadBytes(Stream, 11); break;
                            case 0x0F:
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadBytes(Stream, 1);
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadBytes(Stream, 1);
                                short itemid = StreamHelper.ReadShort(Stream);
                                if (itemid > 0)
                                {
                                    byte amount = StreamHelper.ReadBytes(Stream, 1)[0];
                                    short damage = StreamHelper.ReadShort(Stream);
                                }
                                break;
                            case 0x10: StreamHelper.ReadBytes(Stream, 2); break;
                            case 0x12: StreamHelper.ReadBytes(Stream, 5); break;
                            case 0x13: StreamHelper.ReadBytes(Stream, 5); break;
                            case 0x14:
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadString(Stream);
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadBytes(Stream, 2);
                                StreamHelper.ReadShort(Stream);
                                break;
                            case 0x15: StreamHelper.ReadBytes(Stream, 24); break;
                            case 0x16: StreamHelper.ReadBytes(Stream, 8); break;
                            case 0x17: StreamHelper.ReadBytes(Stream, 17); break;
                            case 0x18:
                                StreamHelper.ReadBytes(Stream, 19);
                                while ((id = (byte)Stream.ReadByte()) != 0x7f) { } //Metadata
                                break;
                            case 0x19:
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadString(Stream);
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadInt(Stream);
                                break;
                            case 0x1C: StreamHelper.ReadBytes(Stream, 10); break;
                            case 0x1D: StreamHelper.ReadBytes(Stream, 4); break;
                            case 0x1E: StreamHelper.ReadBytes(Stream, 4); break;
                            case 0x1F: StreamHelper.ReadBytes(Stream, 7); break;
                            case 0x20: StreamHelper.ReadBytes(Stream, 6); break;
                            case 0x21: StreamHelper.ReadBytes(Stream, 9); break;
                            case 0x22: StreamHelper.ReadBytes(Stream, 18); break;
                            case 0x26: StreamHelper.ReadBytes(Stream, 5); break;
                            case 0x27: StreamHelper.ReadBytes(Stream, 8); break;
                            case 0x28:
                                StreamHelper.ReadInt(Stream);
                                while ((id = (byte)Stream.ReadByte()) != 0x7f) { } //Metadata
                                break;
                            case 0x32: StreamHelper.ReadBytes(Stream, 9); break;
                            case 0x33:
                                StreamHelper.ReadBytes(Stream, 13);
                                int size = StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadBytes(Stream, size);
                                break;
                            case 0x34:
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadInt(Stream);
                                short length = StreamHelper.ReadShort(Stream); //byte array length
                                StreamHelper.ReadBytes(Stream, length * 2); //3 byte arrays
                                while ((id = (byte)Stream.ReadByte()) != 0x7f) { } //Metadata
                                break;
                            case 0x35: StreamHelper.ReadBytes(Stream, 11); break;
                            case 0x3C:
                                StreamHelper.ReadDouble(Stream);
                                StreamHelper.ReadDouble(Stream);
                                StreamHelper.ReadDouble(Stream);
                                StreamHelper.ReadFloat(Stream);
                                int count = StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadBytes(Stream, count * 3);
                                break;
                            case 0x64: StreamHelper.ReadBytes(Stream, 3); break;
                            case 0x65: StreamHelper.ReadBytes(Stream, 1); break;
                            case 0x66: StreamHelper.ReadBytes(Stream, 8); break;
                            case 0x67: StreamHelper.ReadBytes(Stream, 5); break;
                            case 0x68: StreamHelper.ReadBytes(Stream, 3); break;
                            case 0x69: StreamHelper.ReadBytes(Stream, 5); break;
                            case 0x6A: StreamHelper.ReadBytes(Stream, 4); break;
                            case 0x82:
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadShort(Stream);
                                StreamHelper.ReadInt(Stream);
                                StreamHelper.ReadString(Stream);
                                StreamHelper.ReadString(Stream);
                                StreamHelper.ReadString(Stream);
                                break;
                            case 0xFF:
                                String reason = StreamHelper.ReadString(Stream);
                                if (reason.Length < 5) break;
                                Debug.Warning("Received disconnect packet. Reason: " + reason);
                                break;
                            default:
                                //Debug.Warning("Unknown packet received. [" + (int)id + "]");
                                break;
                        }
                    }
                    catch (Exception e) { Debug.Warning(e); }
                }
            }
            catch (Exception e) { Debug.Severe(new MinecraftClientGeneralException(e)); }

            Connected = false;

            Debug.Info("Disconnected from server.");
        }

        private String Authenticate()
        {
            WebClient web = new WebClient();
            String data = web.DownloadString(String.Format("http://www.minecraft.net/game/getversion.jsp?user={0}&password={1}&version={2}", this.Username, this.Password, VERSION));

            if (!data.Contains(":"))
            {
                return "";
            }

            return data.Split(':')[3];
        }

        private Boolean CheckServer()
        {
            WebClient web = new WebClient();
            String data = web.DownloadString(String.Format("http://www.minecraft.net/game/joinserver.jsp?user={0}&sessionId={1}&serverId={2}", this.Username, this.SessionID, this.Server.Hash));

            return (data == "OK");
        }

        private Boolean SendInitialPackets()
        {
            try
            {
                //handshake (client)
                Packets.Types.Handshake clientHandshake = new Packets.Types.Handshake()
                {
                    Username = this.Username
                };
                clientHandshake.Write(this.Stream);

                //handshake (server)
                if (this.Stream.ReadByte() == (byte)Packets.PacketType.Handshake)
                {
                    Packets.Types.Handshake serverHandshake = new Packets.Types.Handshake();
                    serverHandshake.Read(this.Stream);
                    this.Server.Hash = serverHandshake.Username;
                }
                else
                {
                    Debug.Severe(new MinecraftClientConnectException("The server didn't send back the right response"));
                    return false;
                }

                if (this.Server.Hash != "-" && this.Server.Hash != "+")
                {
                    if (this.SessionID == "")
                    {
                        Debug.Severe(new MinecraftClientConnectException("Server requires authenication but it was not enabled."));
                        return false;
                    }

                    if (!CheckServer())
                    {
                        Debug.Severe(new MinecraftClientConnectException("Name verification failed. How you managed this, I don't even know..."));
                        return false;
                    }
                }

                //login (client)
                Packets.Types.Login clientLogin = new Packets.Types.Login()
                {
                    Version = 8,
                    Username = this.Username,
                    ServerPassword = this.Server.Password,
                    MapSeed = 0L,
                    Dimension = 0x00
                };
                clientLogin.Write(this.Stream);

                //login (server)
                if (this.Stream.ReadByte() != (byte)Packets.PacketType.Login)
                {
                    Debug.Severe(new MinecraftClientConnectException("The server didn't send back the right response"));
                    return false;
                }
                Packets.Types.Login serverLogin = new Packets.Types.Login();
                serverLogin.Read(this.Stream);
                this.EntityID = serverLogin.Version;
                this.Server.ServerName = serverLogin.Username;
                this.Server.ServerMOTD = serverLogin.ServerPassword;
                this.Server.MapSeed = serverLogin.MapSeed;

                //this.Stream.WriteByte(0x00);
                //this.Stream.Flush();

                return true;
            }
            catch (Exception e) { Debug.Severe(e); throw e; }
        }

        protected void OnConnectedToServer(object sender, MinecraftClientConnectEventArgs args)
        {
            if (ConnectedToServer != null)
            {
                ConnectedToServer(sender, args);
            }
        }

        protected void OnDisconnectedFromServer(object sender, MinecraftClientConnectEventArgs args)
        {
            if (DisconnectedFromServer != null)
            {
                DisconnectedFromServer(sender, args);
            }
        }

        protected void OnChatMessageReceived(object sender, MinecraftClientChatEventArgs args)
        {
            if (DisconnectedFromServer != null)
            {
                ChatMessageReceived(this, new MinecraftClientChatEventArgs(args.User, args.Message));
            }
        }

        protected void OnPlayerLocationChanged(object sender, MinecraftClientLocationEventArgs args)
        {
            if (PlayerLocationChanged != null)
            {
                PlayerLocationChanged(sender, args);
            }
        }

        public void SetPlayerLocation(Location PlayerLocation) //Currently does nothing
        {
            this.PlayerLocation = PlayerLocation;
            OnPlayerLocationChanged(this, new MinecraftClientLocationEventArgs(this.PlayerLocation));
        }

        public MinecraftServer GetServer()
        {
            return this.Server;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (Stream != null)
                    {
                        Stream.Dispose();
                    }
                    MainSocket.Dispose();

                }

                if (DataThread != null)
                {
                    DataThread.Abort();
                }
                DataThread = null;
                ServerAddress = null;
                Server = null;
                disposed = true;

            }
        }

        ~MCServerConnection()
        {
            Dispose(false);
        }
    }
}
