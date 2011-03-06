using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mcsharpbot.communication;

namespace mcsharpbot.bots
{
    public class MoveBot : MCBotBase
    {

        communication.Entities.NamedEntityType capture;
        System.Timers.Timer positionTimer;
        System.Diagnostics.Stopwatch stopwatch;
        DateTime start;
        List<TimePositionLook> history;
        int currentindex = 0;

        public override void BeginAction()
        {
            Connection.ChatMessageReceived += new communication.MCServerConnection.MinecraftClientChatEventHandler(Connection_ChatMessageReceived);
            Connection.EntityChange += new MCServerConnection.MinecraftClientEntityChange(Connection_EntityChange);
            stopwatch = new System.Diagnostics.Stopwatch();
            positionTimer = new System.Timers.Timer();
            positionTimer.Elapsed += new System.Timers.ElapsedEventHandler(positionTimer_Elapsed);
            history = new List<TimePositionLook>();
            base.BeginAction();
        }

        void positionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (currentindex < history.Count)
            {
                Connection.PlayerLocation.X = history[currentindex].X;
                Connection.PlayerLocation.Y = history[currentindex].Y;
                Connection.PlayerLocation.Z = history[currentindex].Z;
                Connection.PlayerRotation.Pitch = history[currentindex].Pitch;
                Connection.PlayerRotation.Yaw = history[currentindex].Yaw;
                positionTimer.Stop();
                positionTimer.Interval = history[currentindex++].time + 1;
                positionTimer.Start();
            }
            else
            {
                positionTimer.Stop();
            }
        }

        void Connection_EntityChange(communication.Entities.EntityType entity)
        {
            if (capture != null)
            {
                //Debug.Info(String.Format("Pitch: {0}, Yaw:{1}", entity.Pitch, entity.Yaw));
                history.Add(new TimePositionLook()
                {
                    time = stopwatch.ElapsedMilliseconds,
                    X = entity.X,
                    Y = entity.Y,
                    Z = entity.Z,
                    ServerX = entity.ServerX,
                    ServerY = entity.ServerY,
                    ServerZ = entity.ServerZ,
                    Pitch = entity.Pitch,
                    Yaw = entity.Yaw
                });
                stopwatch.Restart();
            }
        }

        void Connection_ChatMessageReceived(object sender, communication.MinecraftClientChatEventArgs args)
        {
            if (!args.User.Equals(Connection.Username))
            {
                if (args.Message.Trim() == "start")
                {
                    if (capture != null)
                    {
                        Connection.SendChat("You cannot start a capture with one already running");
                        return;
                    }
                    currentindex = 0;
                    history.Clear();
                    capture = (communication.Entities.NamedEntityType)Connection.GetServer().Entities.Entities.Where(e => (e.GetType() == typeof(communication.Entities.NamedEntityType)) && (((communication.Entities.NamedEntityType)e).Name == args.User)).First();
                    Connection.SendChat("Starting capture on: " + args.User);
                    start = DateTime.Now;
                    stopwatch.Start();
                }
                if (args.Message.Trim() == "stop")
                {
                    stopwatch.Stop();
                    capture = null;
                    Connection.SendChat("Stopping capture");
                }
                if (args.Message.Trim() == "replay")
                {
                    Connection.SendChat("Replaying movements");
                    currentindex = 0;
                    positionTimer.Interval = 1;
                    positionTimer.Start();
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


        public class TimePositionLook
        {
            public long time;
            public double X;
            public double Y;
            public double Z;
            public int ServerX;
            public int ServerY;
            public int ServerZ;
            public float Yaw;
            public float Pitch;
        }
    }
}
