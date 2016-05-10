using System;
using UserClientServerLib;
using UserCommon;

namespace UserClientLib
{
    public class ServerChatEngine : IChatEngine, ISessionChatEvents
    {
        private UserClientServerLib.UserClientSession userClientSession;
        private IChatEvents chatCreateEvents;
        public ServerChatEngine(UserClientServerLib.UserClientSession userClientSession, IChatEvents chatCreateEvents)
        {
            // TODO: Complete member initialization
            this.userClientSession = userClientSession;
            this.chatCreateEvents = chatCreateEvents;
            this.userClientSession.SessionChatEvents = this;
        }
        public bool IsConnected
        {
            get { return true; }
        }

        public void UserChat(UserCommon.Message message)
        {
            byte[] data = Tool.SerializeHelp.Serialize<Message>(message);
            userClientSession.SendData(UserCommon.UserOP.DoUserChat, data);
        }

        public void OnUserChat(UserCommon.Message message)
        {
            if (chatCreateEvents != null)
            {
                chatCreateEvents.OnUserChat(message);
            }
        }
    }
}
