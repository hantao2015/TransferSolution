using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Text;
using UserCommon;
using System.Linq;

namespace UserClientServerLib
{
    public class UserClientServer : AppServer<UserClientSession, MultiParamBinaryRequestInfo>
    {
        public UserClientServer()
            : base(new DefaultReceiveFilterFactory<UserClientFilter, MultiParamBinaryRequestInfo>())
        {
        }
        private VEventHandler<NewChatEventArgs> m_NewChat;
        public event VEventHandler<NewChatEventArgs> NewChat
        {
            add { m_NewChat += value; }
            remove { m_NewChat -= value; }
        }
        internal void OnNewChat(string newuserSessionId, UserClientSession userClientSession)
        {
            if (m_NewChat == null)
                return;
            var user = UserManage.Instance.GetUser(newuserSessionId);
            if (user == null)
                return;
            m_NewChat(new NewChatEventArgs(user, userClientSession));
        }
    }
    public class NewChatEventArgs : EventArgs
    {
        public NewChatEventArgs(User chatUser, UserClientSession userClientSession)
        {
            ChatUser = chatUser;
            UserClientSession = userClientSession;
        }
        public UserClientSession UserClientSession { get; private set; }
        public User ChatUser { get; private set; }
    }
}
