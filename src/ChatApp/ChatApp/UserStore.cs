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
    public class RoomStore { }
}