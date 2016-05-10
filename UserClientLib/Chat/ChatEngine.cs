using SuperSocket.ClientEngine;
using SuperSocket.ClientEngine.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using UserClientLib.Command;
using UserCommon;

namespace UserClientLib
{
    internal class ChatEngine : IChatEngine, IDisposable
    {
        protected IClientCommandReader<BaseCommandInfo> CommandReader { get; private set; }
        private Dictionary<string, ICommand<ChatEngine, BaseCommandInfo>> m_CommandDict
            = new Dictionary<string, ICommand<ChatEngine, BaseCommandInfo>>(
                StringComparer.OrdinalIgnoreCase);
        internal TcpClientSession Client;
        internal object lockobj = new object();
        public IChatEvents ChatEvents { get; private set; }
        public User CurrentUser { get; private set; }
        internal AutoResetEvent m_OpenedEvent = new AutoResetEvent(false);
        private int m_OPTypeLength;
        private int m_OPCodeLength;
        private int m_ParaCountLength;
        private int m_PerParaCountLength;
        public ChatEngine(IChatEvents chatEvents, User currentUser)
        {
            CurrentUser = currentUser;
            ChatEvents = chatEvents;
            m_OPTypeLength = 1;
            m_OPCodeLength = 1;
            m_ParaCountLength = 1;
            m_PerParaCountLength = 2;
            ChatCommandBase[] baseArray = new ChatCommandBase[] { 
                new DoUserChat(), 
            };
            foreach (ChatCommandBase base2 in baseArray)
            {
                this.m_CommandDict.Add(base2.Name, base2);
            }
            this.CommandReader = new ChatReader(this);
        }

        public void Dispose()
        {
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
        void chatsession_DataReceived(object sender, DataEventArgs e)
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
            ICommand<ChatEngine, BaseCommandInfo> command;
            if (this.m_CommandDict.TryGetValue(commandInfo.Key, out command))
            {
                command.ExecuteCommand(this, commandInfo);
            }
        }
        void chatsession_Connected(object sender, EventArgs e)
        {
            m_OpenedEvent.Set();
        }

        void chatsession_Closed(object sender, EventArgs e)
        {
            //UserChatManage.Instance.RemoveChat(ToUserSessionId);
        }

        void chatsession_Error(object sender, ErrorEventArgs e)
        {
            //UserChatManage.Instance.RemoveChat(ToUserSessionId);
        }

        public void UserChat(UserCommon.Message message)
        {
            byte[] msgData = Tool.SerializeHelp.Serialize<Message>(message);
            this.SendData(UserOP.DoUserChat, msgData);
            log4net.LogManager.GetLogger(this.GetType()).Error(string.Format("我是{0}，我要发送信息给：{1}:{2}", this.CurrentUser.UserName, ToUserAddr, ToUserPort));
        }

        public bool IsConnected
        {
            get { if (Client == null)return false; return Client.IsConnected; }
        }
        int ToUserPort;
        string ToUserAddr;
        public void ChatConn(string toUserAddr, int toUserPort)
        {
            ToUserAddr = toUserAddr;
            ToUserPort = toUserPort;
            EndPoint endpoint = new IPEndPoint(IPAddress.Parse(toUserAddr), toUserPort - 1);
            MyAsyncTcpSession chatsession = new MyAsyncTcpSession(endpoint, 1024 * 10);
            chatsession.Error += chatsession_Error;
            chatsession.DataReceived += chatsession_DataReceived;
            chatsession.Connected += chatsession_Connected;
            chatsession.Closed += chatsession_Closed;
            Client = chatsession;
            Client.Connect();
            if (!m_OpenedEvent.WaitOne(5000))
            {
                throw new Exception("连接超时");
            }
            var currentSessionIdData = Encoding.UTF8.GetBytes(CurrentUser.SessionId);
            this.SendData(UserOP.DoNewChat, currentSessionIdData);
        }
        public void ChatLeave()
        {
            SendData(UserOP.DoChatLeave);
        }
    }
}
