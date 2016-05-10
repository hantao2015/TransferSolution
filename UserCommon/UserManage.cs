using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserCommon
{
    public class UserManage
    {
        object lockobj = new object();
        Dictionary<string, User> Users = new Dictionary<string, User>();
        private UserManage()
        {

        }
        private static class HolderClass
        {
            internal readonly static UserManage instance = new UserManage();
        }
        public static UserManage Instance
        {
            get
            {
                return HolderClass.instance;
            }
        }
        public void AddUser(User user)
        {
            lock (lockobj)
            {
                if (!Users.ContainsKey(user.SessionId))
                {
                    Users.Add(user.SessionId, user);
                }
            }
        }
        public void DelUser(User user)
        {
            lock (lockobj)
            {
                if (Users.ContainsKey(user.SessionId))
                {
                    Users.Remove(user.SessionId);
                }
            }
        }
        public void DelUser(string sessionId)
        {
            lock (lockobj)
            {
                if (Users.ContainsKey(sessionId))
                {
                    Users.Remove(sessionId);
                }
            }
        }
        public void CleareUsers()
        {
            Users.Clear();
        }
        public User[] GetAllUser()
        {
            return Users.Values.ToArray();
        }
        public User GetUser(string sessionId)
        {
            lock (lockobj)
            {
                if (!Users.ContainsKey(sessionId))
                    return null;
                return Users[sessionId];
            }
        }
    }
}
