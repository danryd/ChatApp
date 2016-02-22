using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using NLog;
using NLog.LayoutRenderers.Wrappers;

namespace ChatApp
{
    public class ChatHub : Hub
    {

        private readonly Logger log = LogManager.GetLogger("messageLog");

        public Task Join(string room)
        {
            
            return Groups.Add(Context.ConnectionId, room);

        }

        public Task Leave(string room)
        {
            return Groups.Remove(Context.ConnectionId, room);

        }
        public override Task OnConnected()
        {
          
            UserStore.Add(Context.User.Identity.Name);
            Clients.Caller.currentUsers(UserStore.All);
            Send("Connected", "info");
            return Clients.AllExcept(Context.ConnectionId).joined(Context.User.Identity.Name);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Send("Disconnected", "info");
            UserStore.Remove(Context.User.Identity.Name);
            if (!UserStore.HasAnySession(Context.User.Identity.Name))
                Clients.All.left(Context.User.Identity.Name);
            return base.OnDisconnected(stopCalled);
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
}