using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using NLog;

namespace ChatApp
{
    public class ChatHub : Hub
    {

        private readonly Logger log = LogManager.GetLogger("messageLog");
        public override Task OnConnected()
        {
            new ConnectionMapAccess().Connect(Context.User.Identity.Name, Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            new ConnectionMapAccess().Disconnect(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public Task Join(string room)
        {
            
            var ra= new RoomAccess();
            ra.JoinRoom(room,Context.ConnectionId);
            var connectionsInRoom =ra.ConnectionsInRoom(room);
            var map = new ConnectionMapAccess();
            var users = connectionsInRoom.Select(map.Match);

            Clients.Caller.currentUsers(users);
            Send("Connected", "info");

            Clients.OthersInGroup(room).joined(Context.User.Identity.Name);

            return Groups.Add(Context.ConnectionId, room);

        }

        public Task Leave(string room)
        {
            var ra = new RoomAccess();
            ra.LeaveRoom(room, Context.ConnectionId);

            if (!UserStore.HasAnySession(Context.ConnectionId))
                Clients.OthersInGroup(room).left(Context.User.Identity.Name);
            Send("Disconnected", "info");

            return Groups.Remove(Context.ConnectionId, room);

        }



        public void SendAll(string message, string type, string room)
        {
            var name = Context.User.Identity.Name;
            var time = DateTime.Now.ToString("f");
            Clients.Group(room).All.addNewMessageToPage(name, time, message, type);
        }
        public void Send(string message, string room)
        {
            // Call the addNewMessageToPage method to update clients.
            var name = Context.User.Identity.Name;
            var time = DateTime.Now.ToString("f");
            log.Info($"[{name} @ {time}] {message}");

            if (message.StartsWith("/m "))
            {
                var split = message.Split(' ');
                var user = split[1];
                message = message.Replace("/m ", "");
                message = message.Replace(user, "");
                message = message.TrimStart();
                var targetUser = Clients.User(user);
                if (targetUser == null)
                {
                    Clients.Caller.addNewMessageToPage(name, time, "Ingen sån användare", "error");
                }
                else
                {
                    targetUser.addNewMessageToPage(name, time, message, "private");
                    Clients.Caller.addNewMessageToPage(name, time, message, "private");
                }

            }
            else
            {
                Clients.All.addNewMessageToPage(name, time, message, "regular");
            }

        }
    }

    public class MessageParser
    {
        public MessageParser()
        {

        }
    }
}