using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
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

        public Location PlayerSpawn
        {
            get 
            {
                return _playerSpawn;
            }
            set 
            {
                _playerSpawn = value;
            }
        }
        public IPacket LastReceived
        {
            get { return this.Inbox.Last(); }
        }

        private int EntityID;
        private string SessionID;
        private Location _playerLocation;
        private Location _playerSpawn;
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

        private bool PositionTimerStarted = false;
        private System.Timers.Timer PositionTimer;

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

            PositionTimer = new System.Timers.Timer(100);
            PositionTimer.Elapsed += new ElapsedEventHandler(PositionTimer_Elapsed);
        }


        private void HandleData()
        {
            byte id;
            PacketType type;

            while (MainSocket.Connected && (int)(id = (byte)Stream.ReadByte()) != -1)
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

                        this.PlayerSpawn = new Location(spawnPositionPacket.X, spawnPositionPacket.Y, spawnPositionPacket.Z, spawnPositionPacket.Y + 1.65D);

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

                        this.PlayerLocation.X = playerPositionPacket.X;
                        this.PlayerLocation.Y = playerPositionPacket.Y - 1.6200000047683716D;
                        this.PlayerLocation.Z = playerPositionPacket.Z;
                        this.PlayerLocation.Stance = playerPositionPacket.Stance;
                        this.SendPacket(playerPositionPacket);

                        OnPacketReceived(this, playerPositionPacket);
                        break;
                    case PacketType.PlayerLook:
                        PlayerLook playerLookPacket = new PlayerLook();
                        playerLookPacket.Read(Stream);

                        this.PlayerRotation.Pitch = playerLookPacket.Pitch;
                        this.PlayerRotation.Yaw = playerLookPacket.Yaw;
                        this.SendPacket(playerLookPacket);

                        OnPacketReceived(this, playerLookPacket);
                        break;
                    case PacketType.PlayerLookMove:
                        PlayerLookMove playerLookMovePacket = new PlayerLookMove();
                        playerLookMovePacket.Read(Stream);

                        this.PlayerLocation.X = playerLookMovePacket.X;
                        this.PlayerLocation.Y = playerLookMovePacket.Y - 1.6200000047683716D;
                        this.PlayerLocation.Stance = playerLookMovePacket.Stance;
                        this.PlayerLocation.Z = playerLookMovePacket.Z;
                        this.PlayerRotation.Pitch = playerLookMovePacket.Pitch;
                        this.PlayerRotation.Yaw = playerLookMovePacket.Yaw;
                        this.OnGround = playerLookMovePacket.OnGround;
                        OnPlayerLocationChanged(this, new MinecraftClientLocationEventArgs(this.PlayerLocation));
                        if (!PositionTimerStarted)
                        {
                            PositionTimerStarted = true;
                            PositionTimer.Start();
                        }

                        this.SendPacket(playerLookMovePacket);

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
                            X = ((double)namedEntitySpawnPacket.X / 32D),
                            Y = ((double)namedEntitySpawnPacket.Y / 32D),
                            Z = ((double)namedEntitySpawnPacket.Z / 32D),
                            ServerX = namedEntitySpawnPacket.X,
                            ServerY = namedEntitySpawnPacket.Y,
                            ServerZ = namedEntitySpawnPacket.Z,
                            Pitch = ((float)namedEntitySpawnPacket.Pitch * 360) / 256F,
                            Rotation = ((float)namedEntitySpawnPacket.Rotation * 360) / 256F,
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
                        entityPositonEntity.ServerX += entityPositionPacket.X;
                        entityPositonEntity.ServerY += entityPositionPacket.Y;
                        entityPositonEntity.ServerZ += entityPositionPacket.Z;

                        double entityPositionX = (double)(entityPositonEntity.ServerX) / 32D;
                        double entityPositionY = ((double)(entityPositonEntity.ServerY) / 32D) + 0.015625D;
                        double entityPositionZ = (double)(entityPositonEntity.ServerZ) / 32D;
                        entityPositonEntity.SetPosition(entityPositionX, entityPositionY, entityPositionZ);

                        OnPacketReceived(this, entityPositionPacket);
                        break;
                    case PacketType.EntityLook:
                        EntityLook entityLookPacket = new EntityLook();
                        entityLookPacket.Read(Stream);

                        EntityType entityLookEntity = this.Server.Entities.GetFromId(entityLookPacket.EntityID);
                        if (entityLookEntity == null) break;
                        entityLookEntity.Pitch = (float)(entityLookPacket.Pitch * 360) / 256F;
                        entityLookEntity.Yaw = (float)(entityLookPacket.Yaw * 360) / 256F;

                        OnPacketReceived(this, entityLookPacket);
                        break;
                    case PacketType.EntityLookMove:
                        EntityLookMove entityLookMovePacket = new EntityLookMove();
                        entityLookMovePacket.Read(Stream);

                        EntityType entityLookMoveEntity = this.Server.Entities.GetFromId(entityLookMovePacket.EntityID);
                        if (entityLookMoveEntity == null) break;
                        entityLookMoveEntity.ServerX += entityLookMovePacket.X;
                        entityLookMoveEntity.ServerY += entityLookMovePacket.Y;
                        entityLookMoveEntity.ServerZ += entityLookMovePacket.Z;
                        entityLookMoveEntity.Pitch = (float)(entityLookMovePacket.Pitch * 360) / 256F;
                        entityLookMoveEntity.Yaw = (float)(entityLookMovePacket.Yaw * 360) / 256F;

                        double entityLookMoveX = (double)(entityLookMoveEntity.ServerX) / 32D;
                        double entityLookMoveY = ((double)(entityLookMoveEntity.ServerY) / 32D) + 0.015625D;
                        double entityLookMoveZ = (double)(entityLookMoveEntity.ServerZ) / 32D;
                        entityLookMoveEntity.SetPosition(entityLookMoveX, entityLookMoveY, entityLookMoveZ);

                        OnPacketReceived(this, entityLookMovePacket);
                        break;
                    case PacketType.EntityTeleport:
                        EntityTeleport entityTeleportPacket = new EntityTeleport();
                        entityTeleportPacket.Read(Stream);

                        EntityType entityTeleportEntity = this.Server.Entities.GetFromId(entityTeleportPacket.EntityID);
                        if (entityTeleportEntity == null) break;
                        entityTeleportEntity.ServerX = entityTeleportPacket.X;
                        entityTeleportEntity.ServerY = entityTeleportPacket.Y;
                        entityTeleportEntity.ServerZ = entityTeleportPacket.Z;

                        entityTeleportEntity.SetPosition((double)entityTeleportPacket.X / 32D,
                            (double)(entityTeleportPacket.Y / 32D) + 0.015625D,
                            (double)entityTeleportPacket.Z / 32D);
                        entityTeleportEntity.SetRotation(((float)entityTeleportPacket.Yaw * 360) / 256F,
                            ((float)entityTeleportPacket.Pitch * 360) / 256F);

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
                    case PacketType.EntityMetadata:
                        EntityMetadata entityMetadataPacket = new EntityMetadata();
                        entityMetadataPacket.Read(Stream);
                        OnPacketReceived(this, entityMetadataPacket);
                        break;
                    case PacketType.PreChunk:
                        PreChunk preChunkPacket = new PreChunk();
                        preChunkPacket.Read(Stream);

                        this.Server.Chunks.AllocateChunk(preChunkPacket.X, preChunkPacket.Y);

                        OnPacketReceived(this, preChunkPacket);
                        break;
                    case PacketType.MapChunk:
                        MapChunk mapChunkPacket = new MapChunk();
                        mapChunkPacket.Read(Stream);

                        this.ProcessChunk(mapChunkPacket.X, mapChunkPacket.Y, mapChunkPacket.Z, mapChunkPacket.XSize, mapChunkPacket.YSize, mapChunkPacket.ZSize, mapChunkPacket.Chunk);

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
                        Debug.Warning("Unknown packet received. [" + id.ToString("X2") + "]");
                        break;
                }
            }

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
            string data = web.DownloadString(String.Format("http://www.minecraft.net/game/joinserver.jsp?user={0}&sessionId={1}&serverId={2}", this.Username, this.SessionID, this.Server.Hash));

            web.Dispose();

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
            catch (Exception e) { Debug.Severe(e); throw; }
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

            PlayerLookMove playerLookMove = new PlayerLookMove()
            {
                X = this.PlayerLocation.X,
                Y = this.PlayerLocation.Y,
                Z = this.PlayerLocation.Z,
                Stance = this.PlayerLocation.Stance,
                Pitch = this.PlayerRotation.Pitch,
                Yaw = this.PlayerRotation.Yaw,
                OnGround = false
            };
            this.SendPacket(playerLookMove);

            OnPlayerLocationChanged(this, new MinecraftClientLocationEventArgs(this.PlayerLocation));
        }

        Location PreviousLocation = new Location();
        Rotation PreviousRotation = new Rotation();

        void PositionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!PreviousLocation.Equals(this.PlayerLocation) && !PreviousRotation.Equals(this.PlayerRotation))
            {
                PreviousLocation = (Location)this.PlayerLocation.Clone();
                PreviousRotation = (Rotation)this.PlayerRotation.Clone();
                PlayerLookMove lookMove = new PlayerLookMove()
                {
                    X = this.PlayerLocation.X,
                    Y = this.PlayerLocation.Y,
                    Z = this.PlayerLocation.Z,
                    Stance = this.PlayerLocation.Y + 1.5D,
                    Pitch = this.PlayerRotation.Pitch,
                    Yaw = this.PlayerRotation.Yaw,
                    OnGround = true
                };
                this.SendPacket(lookMove);
                return;
            }
            if (!PreviousLocation.Equals(this.PlayerLocation))
            {
                PreviousLocation = (Location)this.PlayerLocation.Clone();
                PlayerPosition position = new PlayerPosition()
                {
                    X = this.PlayerLocation.X,
                    Y = this.PlayerLocation.Y,
                    Z = this.PlayerLocation.Z,
                    Stance = this.PlayerLocation.Y + 1.5D,
                    OnGround = true
                };
                this.SendPacket(position);
                return;
            }
            if (!PreviousRotation.Equals(this.PlayerRotation))
            {
                PreviousRotation = (Rotation)this.PlayerRotation.Clone();
                PlayerLook look = new PlayerLook()
                {
                    OnGround = true,
                    Pitch = this.PlayerRotation.Pitch,
                    Yaw = this.PlayerRotation.Yaw
                };
                this.SendPacket(look);
                return;
            }
            Flying flying = new Flying()
            {
                OnGround = true
            };
            this.SendPacket(flying);
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

        public void ProcessChunk(int X, int Y, int Z, int XSize, int YSize, int ZSize, byte[] Chunk)
        {
            int xShift = X >> 4;
            int zShift = Z >> 4;
            int xEnd = ((X + XSize) - 1) >> 4;
            int zEnd = ((Z + ZSize) - 1) >> 4;

            int blockSize = 0;
            int yCopy = Y;
            int yEnd = Y + YSize;

            if (yCopy < 0)
            {
                yCopy = 0;
            }
            if (yEnd > 128)
            {
                yEnd = 128;
            }

            for (int i = xShift; i <= xEnd; i++)
            {
                int row = (X - i) * 16;
                int nextrow = ((X + Y) - i) * 16;
                if (row < 0)
                {
                    row = 0;
                }
                if (nextrow > 16)
                {
                    nextrow = 16;
                }

                for (int k = zShift; k <= yEnd; k++)
                {
                    int zRow = (Z - k) * 16;
                    int zNextrow = ((Z + ZSize) - k) * 19;
                    if (zRow < 0)
                    {
                        zRow = 0;
                    }
                    if (zNextrow > 16)
                    {
                        zNextrow = 16;
                    }
                    blockSize = this.Server.Chunks.GetFromCoordinates(i, k).LoadFromChunk(Chunk, row, yCopy, zRow, nextrow, yEnd, zNextrow, blockSize);
                }
            }
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
                    if (PositionTimer != null)
                    {
                        PositionTimer.Dispose();
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
