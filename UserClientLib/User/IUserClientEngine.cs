using SuperSocket.ClientEngine.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UserClientServerLib;
using UserCommon;

namespace UserClientLib
{
    public delegate void UserEventHandler<TEventArgs>(TEventArgs e);
    public interface IUserClientEngine
    {
        event SuperSocket.Common.VEventHandler<UserChatEventArgs> UserChat;
        bool IsConnected { get; }
        IUserEvents UserEvents { get; }
        IClientConfig Config { get; }
        void Login(string UserId, string PassWord);
        void LogOut();
        void Speak(Message message);
        void Whisper(string toUserSessionId, Message msg);
        void SendToServer(Message message);
        void UserStartCreateChat(string touserSessionId);
        User CurrentUser { get; }
    }
    public interface IUserEvents
    {
        void OnLoginComplete(LoginCompleteEventArgs e);
        void OnGetAllUser(GetAllUserEventArgs e);
        void OnGetUser(GetUserEventArgs e);
        void OnUserLeave(UserLeaveEventArgs e);
        void OnUserJoin(UserJoinEventArgs e);
        void OnReceiveMsg(ReceiveMsgEventArgs e);
        void OnUserWhisper(UserWhisperEventArgs e);
        void OnUserStartCreateChat(UserStartCreateChatEventArgs e);
    }
    public class CreateChatEventArgs : EventArgs
    {
        public CreateChatEventArgs(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
        }
        public string Ip { get; private set; }
        public int Port { get; private set; }
    }
    public class UserStartCreateChatEventArgs : EventArgs
    {
        public UserStartCreateChatEventArgs(User user, string ip, int port)
        {
            this.User = user;
            this.Ip = ip;
            this.Port = port;
        }
        public User User { get; private set; }
        public string Ip { get; private set; }
        public int Port { get; private set; }
    }
    public class LoginCompleteEventArgs : EventArgs
    {
        public LoginCompleteEventArgs(bool loginResult, string errormessage)
        {
            this.ErrorMessage = errormessage;
            this.LoginResult = loginResult;
        }
        public string ErrorMessage { get; private set; }
        public bool LoginResult { get; private set; }
    }
    public class GetAllUserEventArgs : EventArgs
    {
        public GetAllUserEventArgs(User[] users)
        {
            Users = users;
        }
        public User[] Users { get; private set; }
    }
    public class GetUserEventArgs : EventArgs
    {
        public GetUserEventArgs(User user)
        {
            User = user;
        }
        public User User { get; private set; }
    }
    public class UserLeaveEventArgs : EventArgs
    {
        public UserLeaveEventArgs(User user)
        {
            User = user;
        }
        public User User { get; private set; }
    }
    public class UserJoinEventArgs : EventArgs
    {
        public UserJoinEventArgs(User user)
        {
            User = user;
        }
        public User User { get; private set; }
    }
    public class ReceiveMsgEventArgs : EventArgs
    {
        public ReceiveMsgEventArgs(User fromUser, Message msg)
        {
            FromUser = fromUser;
            Message = msg;
        }
        public User FromUser { get; private set; }
        public Message Message { get; private set; }
    }
    public class UserWhisperEventArgs : EventArgs
    {
        public UserWhisperEventArgs(User fromUser, Message msg)
        {
            FromUser = fromUser;
            Message = msg;
        }
        public User FromUser { get; private set; }
        public Message Message { get; private set; }
    }
    public class UserChatEventArgs : EventArgs
    {
        public UserChatEventArgs(User chatUser, UserClientSession session)
        {
            UserClientSession = session;
            ChatUser = chatUser;
        }
        public User ChatUser { get; private set; }
        public UserClientSession UserClientSession { get; private set; }
    }
}
