using System.Collections.Generic;
using System.Linq;

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

    public class ConnectionMapAccess
    {
        static ConnectionMapAccess()
        {
            Connections = new Dictionary<string, string>();
        }
        private static readonly IDictionary<string,string> Connections; 
        public void Connect(string username, string connectionId)
        {
            Connections.Add(connectionId,username);
        }

        public void Disconnect(string connectionId)
        {
            Connections.Remove(connectionId);
        }

        public string Match(string connectionId)
        {
            return Connections[connectionId];
        }
    }
    public class RoomAccess
    {
        static RoomAccess()
        {
            RoomMap = new Dictionary<string, List<string>>();
        }
        private static readonly Dictionary<string,List<string>> RoomMap; 
        public string[] ConnectionsInRoom(string room)
        {
            return RoomMap[room].ToArray();
        }
        public void JoinRoom(string room, string connectionId)
        {
            if(!RoomMap.ContainsKey(room))
                RoomMap.Add( room, new List<string>());
            RoomMap[room].Add(connectionId);
        }
        public void LeaveRoom(string room, string connectionId)
        {
            RoomMap[room].Remove(connectionId);
        }

        public string[] RoomsWithUsers()
        {
            return RoomMap.Where(r => r.Value.Count > 0).Select(r=>r.Key).ToArray();
        }
    }
}