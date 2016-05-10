using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserClientServerLib;
using UserCommon;

namespace UserClientLib
{
    public class ChatEngineFactory
    {
        public static IChatEngine CreateChatEngineFromClient(IChatEvents chatCreateEvents, string ip, int port, User currentUser)
        {
            var engine = new ChatEngine(chatCreateEvents, currentUser);
            log4net.LogManager.GetLogger(typeof(ChatEngineFactory)).Error(string.Format("我是：{0}我要连接{1}：{2}", currentUser.UserName, ip, port));
            engine.ChatConn(ip, port);
            return engine;
        }

        public static IChatEngine CreateChatEngineFromServer(UserClientSession userClientSession, IChatEvents chatCreateEvents)
        {
            return new ServerChatEngine(userClientSession, chatCreateEvents);
        }

    }
}
