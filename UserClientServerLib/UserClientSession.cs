using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Text;
using Tool;
using UserCommon;


namespace UserClientServerLib
{
    public class UserClientSession : UserClientSession<UserClientSession>, IDisposable
    {
        internal object lockobj = new object();
        private int m_OPTypeLength;
        private int m_OPCodeLength;
        private int m_ParaCountLength;
        private int m_PerParaCountLength;
        public ISessionChatEvents SessionChatEvents { get; set; }
        public UserClientSession()
        {
            m_OPTypeLength = 1;
            m_OPCodeLength = 1;
            m_ParaCountLength = 1;
            m_PerParaCountLength = 2;
        }
        public void Dispose()
        {
        }
        public void SendData(UserOP opCode, params byte[][] datas)
        {
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
            this.Send(senddata, 0, senddata.Length);
        }

        internal void OnUserChat(Message message)
        {
            if (SessionChatEvents != null)
                SessionChatEvents.OnUserChat(message);
        }
    }

    public interface ISessionChatEvents
    {
        void OnUserChat(Message message);
    }
    public class UserClientSession<TSocketSession> : AppSession<TSocketSession, MultiParamBinaryRequestInfo>,
        IAppSession, ISessionBase where TSocketSession : UserClientSession<TSocketSession>, new()
    {
    }
}
