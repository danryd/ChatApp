using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using NLog;
using NLog.LayoutRenderers.Wrappers;

namespace ChatApp
{
    public static class UserStore
    {
        private static readonly Dictionary<string, int> users = new Dictionary<string, int>();

        public static IEnumerable<string> All
        {
            get { return users.Where(k => k.Value > 0).Select(k => k.Key); }
        }

        public static void Add(string user)
        {
            if (!users.ContainsKey(user))
                users.Add(user, 0);
            users[user]++;
        }
        public static void Remove(string user)
        {
            if (!users.ContainsKey(user))
                return;
            users[user]--;
        }

        public static bool HasAnySession(string user)
        {
            if (!users.ContainsKey(user))
                return false;
            return users[user] > 0;
        }
    }

    public class ChatHub : Hub
    {

        private readonly Logger log = LogManager.GetLogger("messageLog");

        public override Task OnConnected()
        {
            UserStore.Add(Context.User.Identity.Name);
            Clients.Caller.currentUsers(UserStore.All);
            return Clients.AllExcept(Context.ConnectionId).joined(Context.User.Identity.Name);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserStore.Remove(Context.User.Identity.Name);
            if (!UserStore.HasAnySession(Context.User.Identity.Name))
                Clients.All.left(Context.User.Identity.Name);
            return base.OnDisconnected(stopCalled);
        }

        public void Send(string message)
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