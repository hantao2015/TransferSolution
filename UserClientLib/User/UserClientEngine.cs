using SuperSocket.ClientEngine;
using SuperSocket.ClientEngine.Core.Config;
using SuperSocket.ClientEngine.Protocol;
using SuperSocket.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UserClientLib.Command;
using UserClientServerLib;
using UserCommon;

namespace UserClientLib
{
    internal class UserClientEngine : IUserClientEngine, IDisposable
    {
        protected IClientCommandReader<BaseCommandInfo> CommandReader { get; private set; }
        private Dictionary<string, ICommand<UserClientEngine, BaseCommandInfo>> m_CommandDict
            = new Dictionary<string, ICommand<UserClientEngine, BaseCommandInfo>>(
                StringComparer.OrdinalIgnoreCase);
        internal TcpClientSession Client;
        internal object lockobj = new object();
        public IUserEvents UserEvents { get; private set; }
        public IClientConfig Config { get; set; }
        internal AutoResetEvent m_OpenedEvent = new AutoResetEvent(false);
        private int m_OPTypeLength;
        private int m_OPCodeLength;
        private int m_ParaCountLength;
        private int m_PerParaCountLength;
        public User CurrentUser { get; private set; }
        #region 构造函数
        public UserClientEngine(IUserEvents userEvents)
        {
            UserEvents = userEvents;
            var TotalConfig = ConfigurationManager.GetSection("superSocket") as IConfigurationSource;
            IClientConfig config = TotalConfig.Clients.FirstOrDefault();
            Init(config);
        }
        public UserClientEngine(IUserEvents userEvents, EndPoint endpoint, int MaxReceiveLength)
        {
            UserEvents = userEvents;
            Init(endpoint, MaxReceiveLength);
        }
        public UserClientEngine(IUserEvents userEvents, IClientConfig config)
        {
            UserEvents = userEvents;
            Config = config;
            Init();
        }
        #endregion
        #region 初始化
        public void Init(EndPoint endpoint, int MaxReceiveLength)
        {
            m_OPTypeLength = 1;
            m_OPCodeLength = 1;
            m_ParaCountLength = 1;
            m_PerParaCountLength = 2;
            UserCommandBase[] baseArray = new UserCommandBase[] { 
                new DoLoginResult(), 
                new DoUserJoin(), 
                new DoGetAllUser(), 
                new DoGetUser(), 
                new DoWhisper(), 
                new DoBroadCast(), 
                new DoUserLeave(),
                new DoUserStartCreateChat(),
            };
            foreach (UserCommandBase base2 in baseArray)
            {
                this.m_CommandDict.Add(base2.Name, base2);
            }
            MyAsyncTcpSession session = new MyAsyncTcpSession(endpoint, MaxReceiveLength);
            session.Error += session_Error;
            session.DataReceived += session_DataReceived;
            session.Connected += session_Connected;
            session.Closed += session_Closed;
            session.GetSocket += session_GetSocket;
            this.Client = session;
            this.CommandReader = new UserReader(this);
        }

        void session_GetSocket(GetSocketEventArgs e)
        {
            m_UserClientServer = new UserClientServer();
            var ipendpoint = (System.Net.IPEndPoint)e.LocalEndPoint;
            m_UserClientServer.Setup(ipendpoint.Address.ToString(), ipendpoint.Port, null, null, null, null);
            m_UserClientServer.NewChat += m_UserClientServer_NewChat;
            bool result = m_UserClientServer.Start();
           
            if (!result)
            {
                //debughere
            }
            else
            {
              //  log4net.LogManager.GetLogger(this.GetType()).Error(string.Format("{0}:{1}", this.CurrentUser.UserName, ipendpoint.Port));
            }
        }

        void m_UserClientServer_NewChat(NewChatEventArgs e)
        {
            var handle = m_UserChat;
            if (m_UserChat != null)
                m_UserChat(new UserChatEventArgs(e.ChatUser, e.UserClientSession));
        }
        public void Init(IClientConfig config)
        {
            Config = config;
            Init();
        }
        public void Init()
       {
            IPHostEntry ipHost = Dns.GetHostEntry(Config.Ip);
            IPEndPoint endpoint = new IPEndPoint(ipHost.AddressList[0], Config.Port);
            //EndPoint endpoint = new IPEndPoint(IPAddress.Parse(Config.Ip), Config.Port);
            Init(endpoint, Config.MaxReceiveLength);
        }
        #endregion
        void session_Closed(object sender, EventArgs e)
        {
            UserManage.Instance.CleareUsers();
        }
        void session_Connected(object sender, EventArgs e)
        {
            this.m_OpenedEvent.Set();
        }
        void session_DataReceived(object sender, DataEventArgs e)
        {
            this.m_OnDataReceived(e.Data, e.Offset, e.Length);
        }
        private void m_OnDataReceived(byte[] data, int offset, int length)
        {
            while (true)
            {
                if (this.CommandReader == null)
                    return;
                int rest;
                BaseCommandInfo commandInfo = this.CommandReader.GetCommandInfo(data, offset, length, out rest);
                if (this.CommandReader.NextCommandReader != null)
                {
                    this.CommandReader = this.CommandReader.NextCommandReader;
                }
                if (commandInfo != null)
                {
                    this.ExecuteCommand(commandInfo);
                }
                if (rest <= 0)
                    return;
                offset = (offset + length) - rest;
                length = rest;
            }
        }
        private void ExecuteCommand(BaseCommandInfo commandInfo)
        {
            ICommand<UserClientEngine, BaseCommandInfo> command;
            if (this.m_CommandDict.TryGetValue(commandInfo.Key, out command))
            {
                command.ExecuteCommand(this, commandInfo);
            }
        }
        void session_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
        }
        public void Dispose()
        {
        }
        public void Login(string UserId, string PassWord)
        {
            
           Client.Connect();
            if (!m_OpenedEvent.WaitOne(5000))
            {
                throw new Exception("连接超时");
            }
            byte[] userIddata = Encoding.UTF8.GetBytes(UserId);
            byte[] pwddata = Encoding.UTF8.GetBytes(PassWord);
            SendData(UserOP.DoLogin, userIddata, pwddata);
            
        }
        internal void SendData(UserOP opCode, params byte[][] datas)
        {
            if (this.Client == null)
                return;
            int totalLen = 0;
            foreach (byte[] data in datas)
            {
                totalLen = totalLen + data.Length;
            }
            //if (datas.Length == 0)
            //{
            //    totalLen = totalLen + 1;
            //}
            totalLen = m_OPTypeLength + m_OPCodeLength + m_ParaCountLength + datas.Length * m_PerParaCountLength + totalLen;
            int flag = 0;
            int optype = 0;
            byte[] senddata = new byte[totalLen];
            if (m_OPTypeLength == 1)
            {
                senddata[flag] = (byte)((optype & 0xFF));
            }
            else if (m_OPTypeLength == 2)
            {
                senddata[flag + 1] = (byte)((optype >> 8) & 0xFF);
                senddata[flag] = (byte)((optype & 0xFF));
            }
            else if (m_OPTypeLength == 3)
            {
                senddata[flag + 2] = (byte)((optype >> 16) & 0xFF);
                senddata[flag + 1] = (byte)((optype >> 8) & 0xFF);
                senddata[flag] = (byte)((optype & 0xFF));
            }
            else
            {
                senddata[flag + 3] = (byte)((optype >> 24) & 0xFF);
                senddata[flag + 2] = (byte)((optype >> 16) & 0xFF);
                senddata[flag + 1] = (byte)((optype >> 8) & 0xFF);
                senddata[flag] = (byte)((optype & 0xFF));
            }
            flag = flag + m_OPTypeLength;
            if (m_OPCodeLength == 1)
            {
                senddata[flag] = (byte)(((int)opCode & 0xFF));
            }
            else if (m_OPCodeLength == 2)
            {
                senddata[flag + 1] = (byte)(((int)opCode >> 8) & 0xFF);
                senddata[flag] = (byte)(((int)opCode & 0xFF));
            }
            else if (m_OPCodeLength == 3)
            {
                senddata[flag + 2] = (byte)(((int)opCode >> 16) & 0xFF);
                senddata[flag + 1] = (byte)(((int)opCode >> 8) & 0xFF);
                senddata[flag] = (byte)(((int)opCode & 0xFF));
            }
            else
            {
                senddata[flag + 3] = (byte)(((int)opCode >> 24) & 0xFF);
                senddata[flag + 2] = (byte)(((int)opCode >> 16) & 0xFF);
                senddata[flag + 1] = (byte)(((int)opCode >> 8) & 0xFF);
                senddata[flag] = (byte)(((int)opCode & 0xFF));

            }
            flag = flag + m_OPCodeLength;
            if (m_ParaCountLength == 1)
            {
                senddata[flag] = (byte)((datas.Length & 0xFF));
            }
            else if (m_ParaCountLength == 2)
            {
                senddata[flag + 1] = (byte)((datas.Length >> 8) & 0xFF);
                senddata[flag] = (byte)((datas.Length & 0xFF));
            }
            else if (m_ParaCountLength == 3)
            {
                senddata[flag + 2] = (byte)((datas.Length >> 16) & 0xFF);
                senddata[flag + 1] = (byte)((datas.Length >> 8) & 0xFF);
                senddata[flag] = (byte)((datas.Length & 0xFF));
            }
            else
            {
                senddata[flag + 3] = (byte)((datas.Length >> 24) & 0xFF);
                senddata[flag + 2] = (byte)((datas.Length >> 16) & 0xFF);
                senddata[flag + 1] = (byte)((datas.Length >> 8) & 0xFF);
                senddata[flag] = (byte)((datas.Length & 0xFF));
            }
            flag = flag + m_ParaCountLength;
            for (int i = 0; i < datas.Length; i++)
            {
                if (m_PerParaCountLength == 1)
                {
                    senddata[flag] = (byte)((datas[i].Length & 0xFF));
                }
                else if (m_PerParaCountLength == 2)
                {
                    senddata[flag + 1] = (byte)((datas[i].Length >> 8) & 0xFF);
                    senddata[flag] = (byte)((datas[i].Length & 0xFF));
                }
                else if (m_PerParaCountLength == 3)
                {
                    senddata[flag + 2] = (byte)((datas[i].Length >> 16) & 0xFF);
                    senddata[flag + 1] = (byte)((datas[i].Length >> 8) & 0xFF);
                    senddata[flag] = (byte)((datas[i].Length & 0xFF));
                }
                else
                {
                    senddata[flag + 3] = (byte)((datas[i].Length >> 24) & 0xFF);
                    senddata[flag + 2] = (byte)((datas[i].Length >> 16) & 0xFF);
                    senddata[flag + 1] = (byte)((datas[i].Length >> 8) & 0xFF);
                    senddata[flag] = (byte)((datas[i].Length & 0xFF));
                }
                flag = flag + m_PerParaCountLength;
                Buffer.BlockCopy(datas[i], 0, senddata, flag, datas[i].Length);
                flag = flag + datas[i].Length;
            }
            this.Client.Send(senddata, 0, senddata.Length);
        }

        public void LogOut()
        {
            SendData(UserOP.DoLogOut);
        }

        public void Whisper(string toUserSessionId, UserCommon.Message msg)
        {
            var msgData = Tool.SerializeHelp.Serialize<Message>(msg);
            var toUserData = Encoding.UTF8.GetBytes(toUserSessionId);
            SendData(UserOP.DoWhisper, toUserData, msgData);
        }

        internal void OnLoginComplete(bool result, string errorMsg)
        {
            if (UserEvents != null)
            {
                this.UserEvents.OnLoginComplete(new LoginCompleteEventArgs(result, errorMsg));
            }
        }

        internal void OnGetAllUser(List<User> users)
        {
            if (UserEvents != null)
            {
                this.UserEvents.OnGetAllUser(new GetAllUserEventArgs(users.ToArray()));
            }
        }

        internal void OnGetUser(User user)
        {
            this.CurrentUser = user;
            if (UserEvents != null)
            {
                this.UserEvents.OnGetUser(new GetUserEventArgs(user));
            }
        }

        public bool IsConnected
        {
            get { if (Client == null)return false; return Client.IsConnected; }
        }

        public void Speak(Message message)
        {
            var msgData = Tool.SerializeHelp.Serialize<Message>(message);
            SendData(UserOP.DoBroadCast, msgData);
        }

        public void SendToServer(Message message)
        {
            var msgData = Tool.SerializeHelp.Serialize<Message>(message);
            SendData(UserOP.DoSendToServer, msgData);
        }

        public void UserStartCreateChat(string touserSessionId)
        {
            var toUserData = Encoding.UTF8.GetBytes(touserSessionId);
            SendData(UserOP.DoUserStartCreateChat, toUserData);
        }

        private VEventHandler<UserChatEventArgs> m_UserChat;
        public event VEventHandler<UserChatEventArgs> UserChat
        {
            add { m_UserChat += value; }
            remove { m_UserChat -= value; }
        }
        internal void OnUserStartCreateChat(string toUserSessionId, string toUserAddr, int toUserPort)
        {
            if (UserEvents != null)
            {
                var user = UserManage.Instance.GetUser(toUserSessionId);
                this.UserEvents.OnUserStartCreateChat(
                    new UserStartCreateChatEventArgs(user, toUserAddr, toUserPort));
            }
        }

        UserClientServer m_UserClientServer;
    }
}
