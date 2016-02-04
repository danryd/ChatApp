using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.SignalR;
using NLog;
using NLog.LayoutRenderers.Wrappers;

namespace ChatApp
{

    
    public class ChatHub : Hub
    {
        private readonly Logger log = LogManager.GetLogger("messageLog");
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
                var targetUser  = Clients.User(user);
                if (targetUser == null)
                {
                    Clients.Caller.addNewMessageToPage(name, time, "Ingen sån användare", "error");
                }
                else
                {
                    targetUser.addNewMessageToPage(name, time, message,"private");
                    Clients.Caller.addNewMessageToPage(name, time, message, "private");
                }
                
            }
            else
            {
                  Clients.All.addNewMessageToPage(name,time, message, "regular");
            }
          
        }
    }
}