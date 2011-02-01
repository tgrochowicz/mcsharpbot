using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using mcsharpbot.communication.Packets;
using mcsharpbot.communication.Packets.Types;
using mcsharpbot.communication.Entities;

namespace mcsharpbot.communication
{
    public class MCServerConnection : IDisposable
    {
        public string Username;
        private string Password;
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
        public delegate void MinecraftClientPacketTransmission(object sender, IPacket packet);
        public event MinecraftClientConnectEventHandler ConnectedToServer, DisconnectedFromServer;
        public event MinecraftClientChatEventHandler ChatMessageReceived;
        public event MinecraftClientLocationEventHandler PlayerLocationChanged;
        public event MinecraftClientPacketTransmission PacketReceived, PacketSent;

        public Thread DataThread;
        private bool disposed = false;

        private List<IPacket> Inbox = new List<IPacket>();
        private List<IPacket> Outbox = new List<IPacket>();

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
            PacketType type;

            try
            {
                while (MainSocket.Connected && (int)(id = (byte)Stream.ReadByte()) != -1)
                {
                    try
                    {
                        type = (PacketType)id;
                        switch (type)
                        {
                            case PacketType.Ping: //Ping
                                Ping pingPacket = new Ping();
                                this.SendPacket(pingPacket);
                                OnPacketReceived(this, pingPacket);
                                break;
                            case PacketType.Chat: //Chat
                                try
                                {
                                    Chat chatPacket = new Chat();
                                    chatPacket.Read(Stream);
                                    string chatMsg = chatPacket.Message;
                                    string[] chatMsgSplit = chatMsg.Split(new char[] { '>', ']' });
                                    string chatUsername = chatMsgSplit[0].Trim('<', '[');
                                    if (this.Username == chatUsername)
                                    {
                                        break;
                                    }
                                    OnChatMessageReceived(this, new MinecraftClientChatEventArgs(chatUsername, chatMsgSplit[1]));
                                    OnPacketReceived(this, chatPacket);
                                }
                                catch (IndexOutOfRangeException e) { }
                                break;
                            case PacketType.UpdateTime:
                                UpdateTime updateTimePacket = new UpdateTime();
                                updateTimePacket.Read(Stream);
                                this.Server.Time = updateTimePacket.Time;
                                OnPacketReceived(this, updateTimePacket);
                                break;
                            case PacketType.PlayerInventory:
                                PlayerInventory playerInventoryPacket = new PlayerInventory();
                                playerInventoryPacket.Read(Stream);
                                OnPacketReceived(this, playerInventoryPacket);
                                break;
                            case PacketType.SpawnPosition:
                                SpawnPosition spawnPositionPacket = new SpawnPosition();
                                spawnPositionPacket.Read(Stream);
                                OnPacketReceived(this, spawnPositionPacket);
                                break;
                            case PacketType.Use:
                                Use usePacket = new Use();
                                usePacket.Read(Stream);
                                OnPacketReceived(this, usePacket);
                                break;
                            case PacketType.Health:
                                Health healthPacket = new Health();
                                healthPacket.Read(Stream);
                                OnPacketReceived(this, healthPacket);
                                break;
                            case PacketType.Respawn:
                                Respawn respawnPacket = new Respawn();
                                respawnPacket.Read(Stream);
                                OnPacketReceived(this, respawnPacket);
                                break;
                            case PacketType.Flying:
                                Flying flyingPacket = new Flying();
                                flyingPacket.Read(Stream);
                                OnPacketReceived(this, flyingPacket);
                                break;
                            case PacketType.PlayerPosition:
                                PlayerPosition playerPositionPacket = new PlayerPosition();
                                playerPositionPacket.Read(Stream);
                                OnPacketReceived(this, playerPositionPacket);
                                break;
                            case PacketType.PlayerLook:
                                PlayerLook playerLookPacket = new PlayerLook();
                                playerLookPacket.Read(Stream);
                                this.PlayerRotation.Pitch = playerLookPacket.Pitch;
                                this.PlayerRotation.Yaw = playerLookPacket.Yaw;
                                OnPacketReceived(this, playerLookPacket);
                                break;
                            case PacketType.PlayerLookMove:
                                PlayerLookMove playerLookMovePacket = new PlayerLookMove();
                                playerLookMovePacket.Read(Stream);
                                this.PlayerLocation.X = playerLookMovePacket.X;
                                this.PlayerLocation.Y = playerLookMovePacket.Y;
                                this.PlayerLocation.Stance = playerLookMovePacket.Stance;
                                this.PlayerLocation.Z = playerLookMovePacket.Z;
                                this.PlayerRotation.Pitch = playerLookMovePacket.Pitch;
                                this.PlayerRotation.Yaw = playerLookMovePacket.Yaw;
                                this.OnGround = playerLookMovePacket.OnGround;
                                OnPlayerLocationChanged(this, new MinecraftClientLocationEventArgs(this.PlayerLocation));
                                OnPacketReceived(this, playerLookMovePacket);
                                break;
                            case PacketType.BlockDig:
                                BlockDig blockDigPacket = new BlockDig();
                                blockDigPacket.Read(Stream);
                                OnPacketReceived(this, blockDigPacket);
                                break;
                            case PacketType.Place:
                                Place placePacket = new Place();
                                placePacket.Read(Stream);
                                OnPacketReceived(this, placePacket);
                                break;
                            case PacketType.BlockItemSwitch:
                                BlockItemSwitch blockItemSwitchPacket = new BlockItemSwitch();
                                blockItemSwitchPacket.Read(Stream);
                                OnPacketReceived(this, blockItemSwitchPacket);
                                break;
                            case PacketType.ArmAnimation:
                                ArmAnimation armAnimationPacket = new ArmAnimation();
                                armAnimationPacket.Read(Stream);
                                OnPacketReceived(this, armAnimationPacket);
                                break;
                            case PacketType.Action:
                                Packets.Types.Action actionPacket = new Packets.Types.Action();
                                actionPacket.Read(Stream);
                                OnPacketReceived(this, actionPacket);
                                break;
                            case PacketType.NamedEntitySpawn:
                                NamedEntitySpawn namedEntitySpawnPacket = new NamedEntitySpawn();
                                namedEntitySpawnPacket.Read(Stream);

                                NamedEntityType namedEntitySpawnEntity = new NamedEntityType()
                                {
                                    EntityID = namedEntitySpawnPacket.EntityID,
                                    X = namedEntitySpawnPacket.X,
                                    Y = namedEntitySpawnPacket.Y,
                                    Z = namedEntitySpawnPacket.Z,
                                    Pitch = namedEntitySpawnPacket.Pitch,
                                    Rotation = namedEntitySpawnPacket.Rotation,
                                    CurrentItem = namedEntitySpawnPacket.CurrentItem,
                                    Name = namedEntitySpawnPacket.Name
                                };
                                this.Server.Entities.Add(namedEntitySpawnEntity);

                                OnPacketReceived(this, namedEntitySpawnPacket);
                                break;
                            case PacketType.PickupSpawn:
                                PickupSpawn pickupSpawnPacket = new PickupSpawn();
                                pickupSpawnPacket.Read(Stream);

                                PickupEntityType pickupSpawnEntity = new PickupEntityType()
                                {
                                    EntityID = pickupSpawnPacket.EntityID,
                                    X = pickupSpawnPacket.X,
                                    Y = pickupSpawnPacket.Y,
                                    Z = pickupSpawnPacket.Z,
                                    Pitch = pickupSpawnPacket.Pitch,
                                    ItemID = pickupSpawnPacket.ItemID,
                                    Count = pickupSpawnPacket.Count,
                                    Rotation = pickupSpawnPacket.Rotation,
                                    Secondary = pickupSpawnPacket.Secondary,
                                    Roll = pickupSpawnPacket.Roll
                                };
                                this.Server.Entities.Add(pickupSpawnEntity);

                                OnPacketReceived(this, pickupSpawnPacket);
                                break;
                            case PacketType.Collect:
                                Collect collectPacket = new Collect();
                                collectPacket.Read(Stream);
                                OnPacketReceived(this, collectPacket);
                                break;
                            case PacketType.VehicleSpawn:
                                VehicleSpawn vehicleSpawnPacket = new VehicleSpawn();
                                vehicleSpawnPacket.Read(Stream);

                                VehicleEntityType vehicleSpawnEntity = new VehicleEntityType()
                                {
                                    EntityID = vehicleSpawnPacket.EntityID,
                                    X = vehicleSpawnPacket.X,
                                    Y = vehicleSpawnPacket.Y,
                                    Z = vehicleSpawnPacket.Z,
                                    VehicleType = vehicleSpawnPacket.VehicleType
                                };
                                this.Server.Entities.Add(vehicleSpawnEntity);

                                OnPacketReceived(this, vehicleSpawnPacket);
                                break;
                            case PacketType.MobSpawn:
                                MobSpawn mobSpawnPacket = new MobSpawn();
                                mobSpawnPacket.Read(Stream);

                                MobEntityType mobSpawnEntity = new MobEntityType()
                                {
                                    EntityID = mobSpawnPacket.EntityID,
                                    X = mobSpawnPacket.X,
                                    Y = mobSpawnPacket.Y,
                                    Z = mobSpawnPacket.Z,
                                    Pitch = mobSpawnPacket.Pitch,
                                    Yaw = mobSpawnPacket.Yaw,
                                    MobType = mobSpawnPacket.MobType
                                };
                                this.Server.Entities.Add(mobSpawnEntity);

                                OnPacketReceived(this, mobSpawnPacket);
                                break;
                            case PacketType.Painting:
                                Painting paintingPacket = new Painting();
                                paintingPacket.Read(Stream);
                                OnPacketReceived(this, paintingPacket);
                                break;
                            case PacketType.Velocity:
                                Velocity velocityPacket = new Velocity();
                                velocityPacket.Read(Stream);
                                OnPacketReceived(this, velocityPacket);
                                break;
                            case PacketType.DestroyEntity:
                                DestroyEntity destroyEntityPacket = new DestroyEntity();
                                destroyEntityPacket.Read(Stream);

                                this.Server.Entities.Remove(destroyEntityPacket.EntityID);

                                OnPacketReceived(this, destroyEntityPacket);
                                break;
                            case PacketType.Entity:
                                Entity entityPacket = new Entity();
                                entityPacket.Read(Stream);

                                EntityType entityEntityType = new EntityType()
                                {
                                    EntityID = entityPacket.EntityID
                                };
                                this.Server.Entities.Add(entityEntityType);

                                OnPacketReceived(this, entityPacket);
                                break;
                            case PacketType.EntityPosition:
                                EntityPosition entityPositionPacket = new EntityPosition();
                                entityPositionPacket.Read(Stream);

                                EntityType entityPositonEntity = this.Server.Entities.GetFromId(entityPositionPacket.EntityID);
                                if (entityPositonEntity == null) break;
                                entityPositonEntity.X = entityPositionPacket.X;
                                entityPositonEntity.Y = entityPositionPacket.Y;
                                entityPositonEntity.Z = entityPositionPacket.Z;

                                OnPacketReceived(this, entityPositionPacket);
                                break;
                            case PacketType.EntityLook:
                                EntityLook entityLookPacket = new EntityLook();
                                entityLookPacket.Read(Stream);

                                EntityType entityLookEntity = this.Server.Entities.GetFromId(entityLookPacket.EntityID);
                                if (entityLookEntity == null) break;
                                entityLookEntity.Pitch = entityLookPacket.Pitch;
                                entityLookEntity.Yaw = entityLookPacket.Yaw;

                                OnPacketReceived(this, entityLookPacket);
                                break;
                            case PacketType.EntityLookMove:
                                EntityLookMove entityLookMovePacket = new EntityLookMove();
                                entityLookMovePacket.Read(Stream);

                                EntityType entityLookMoveEntity = this.Server.Entities.GetFromId(entityLookMovePacket.EntityID);
                                if (entityLookMoveEntity == null) break;
                                entityLookMoveEntity.X = entityLookMovePacket.X;
                                entityLookMoveEntity.Y = entityLookMovePacket.Y;
                                entityLookMoveEntity.Z = entityLookMovePacket.Z;
                                entityLookMoveEntity.Pitch = entityLookMovePacket.Pitch;
                                entityLookMoveEntity.Yaw = entityLookMovePacket.Yaw;

                                OnPacketReceived(this, entityLookMovePacket);
                                break;
                            case PacketType.EntityTeleport:
                                EntityTeleport entityTeleportPacket = new EntityTeleport();
                                entityTeleportPacket.Read(Stream);

                                EntityType entityTeleportEntity = this.Server.Entities.GetFromId(entityTeleportPacket.EntityID);
                                if (entityTeleportEntity == null) break;
                                entityTeleportEntity.X = entityTeleportPacket.X;
                                entityTeleportEntity.Y = entityTeleportPacket.Y;
                                entityTeleportEntity.Z = entityTeleportPacket.Z;
                                entityTeleportEntity.Pitch = entityTeleportPacket.Pitch;
                                entityTeleportEntity.Yaw = entityTeleportPacket.Yaw;

                                OnPacketReceived(this, entityTeleportPacket);
                                break;
                            case PacketType.Status:
                                Status statusPacket = new Status();
                                statusPacket.Read(Stream);
                                OnPacketReceived(this, statusPacket);
                                break;
                            case PacketType.VehicleAttach:
                                VehicleAttach vehicleAttachPacket = new VehicleAttach();
                                vehicleAttachPacket.Read(Stream);
                                OnPacketReceived(this, vehicleAttachPacket);
                                break;
                            /*case 0x28:
                                StreamHelper.ReadInt(Stream);
                                while ((id = (byte)Stream.ReadByte()) != 0x7f) { } //Metadata
                                break;*/
                            case PacketType.PreChunk:
                                PreChunk preChunkPacket = new PreChunk();
                                preChunkPacket.Read(Stream);
                                OnPacketReceived(this, preChunkPacket);
                                break;
                            case PacketType.MapChunk:
                                MapChunk mapChunkPacket = new MapChunk();
                                mapChunkPacket.Read(Stream);
                                OnPacketReceived(this, mapChunkPacket);
                                break;
                            case PacketType.MultiBlockChange:
                                MultiBlockChange multiBlockChangePacket = new MultiBlockChange();
                                multiBlockChangePacket.Read(Stream);
                                OnPacketReceived(this, multiBlockChangePacket);
                                break;
                            case PacketType.BlockChange:
                                BlockChange blockChangePacket = new BlockChange();
                                blockChangePacket.Read(Stream);
                                OnPacketReceived(this, blockChangePacket);
                                break;
                            case PacketType.Note:
                                Note notePacket = new Note();
                                notePacket.Read(Stream);
                                OnPacketReceived(this, notePacket);
                                break;
                            case PacketType.Explosion:
                                Explosion explosionPacket = new Explosion();
                                explosionPacket.Read(Stream);
                                OnPacketReceived(this, explosionPacket);
                                break;
                            case PacketType.WindowOpen:
                                WindowOpen windowOpenPacket = new WindowOpen();
                                windowOpenPacket.Read(Stream);
                                OnPacketReceived(this, windowOpenPacket);
                                break;
                            case PacketType.WindowClose:
                                WindowClose windowClosePacket = new WindowClose();
                                windowClosePacket.Read(Stream);
                                OnPacketReceived(this, windowClosePacket);
                                break;
                            case PacketType.WindowAction:
                                WindowAction windowActionPacket = new WindowAction();
                                windowActionPacket.Read(Stream);
                                OnPacketReceived(this, windowActionPacket);
                                break;
                            case PacketType.WindowSlot:
                                WindowSlot windowSlotPacket = new WindowSlot();
                                windowSlotPacket.Read(Stream);
                                OnPacketReceived(this, windowSlotPacket);
                                break;
                            case PacketType.Inventory:
                                Inventory inventoryPacket = new Inventory();
                                inventoryPacket.Read(Stream);
                                OnPacketReceived(this, inventoryPacket);
                                break;
                            case PacketType.WindowProgress:
                                WindowProgress windowProgressPacket = new WindowProgress();
                                windowProgressPacket.Read(Stream);
                                OnPacketReceived(this, windowProgressPacket);
                                break;
                            case PacketType.WindowToken:
                                WindowToken windowTokenPacket = new WindowToken();
                                windowTokenPacket.Read(Stream);
                                OnPacketReceived(this, windowTokenPacket);
                                break;
                            case PacketType.Sign:
                                Sign signPacket = new Sign();
                                signPacket.Read(Stream);
                                OnPacketReceived(this, signPacket);
                                break;
                            case PacketType.Quit:
                                Quit quitPacket = new Quit();
                                quitPacket.Read(Stream);
                                Debug.Warning("Received disconnect packet. Reason: " + quitPacket.Reason);
                                OnPacketReceived(this, quitPacket);
                                break;
                            default:
                                //Debug.Warning("Unknown packet received. [" + (int)id + "]");
                                break;
                        }
                    }
                    catch (Exception e) { Debug.Warning(e); }
                }
            }
            catch (Exception e) { Debug.Severe(new MinecraftClientGeneralException(e)); throw e; }

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
                this.SendPacket(clientHandshake);

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
                this.SendPacket(clientLogin);

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

                return true;
            }
            catch (Exception e) { Debug.Severe(e); throw e; }
        }

        protected void OnPacketReceived(object sender, IPacket packet)
        {
        #if DEBUG
            if (packet.Type != PacketType.Ping) Inbox.Add(packet);
        #endif
            if (PacketReceived != null)
            {
                PacketReceived(sender, packet);
            }
        }

        protected void OnPacketSent(object sender, IPacket packet)
        {
        #if DEBUG
            if (packet.Type != PacketType.Ping) Outbox.Add(packet);
        #endif
            if (PacketSent != null)
            {
                PacketSent(sender, packet);
            }
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
            if (ChatMessageReceived != null)
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

        public void SendPacket(IPacket packet)
        {
            packet.Write(Stream);
            OnPacketSent(this, packet);
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

                if (DataThread != null)
                {
                    DataThread.Abort();
                }
                if (disposing)
                {
                    // Dispose managed resources.
                    if (Stream != null)
                    {
                        Stream.Dispose();
                    }
                    if (MainSocket != null)
                    {
                        MainSocket.Dispose();
                    }

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
