using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Text;
using UserCommon;
using System.Linq;
namespace UserServerLib
{
    public class UserServer : AppServer<UserSession, MultiParamBinaryRequestInfo>
    {
        public event VEventHandler<UserJoinEventArgs> UserJoin;
        public event VEventHandler<UserLeaveEventArgs> UserLeave;
        public event VEventHandler<BroadcastEventArgs> Broadcast;
        public event VEventHandler<UserWhisperEventArgs> UserWhisper;
        public event VEventHandler<SendToServerEventArgs> SendToServer;
        public UserServer()
            : base(new DefaultReceiveFilterFactory<UserFilter, MultiParamBinaryRequestInfo>())
        {
        }
        internal void OnSendToServer(User fromUser, Message msg)
        {
            if (SendToServer != null)
            {
                this.SendToServer(new SendToServerEventArgs(fromUser, msg));
            }
        }
        internal void OnBroadCast(User fromUser, Message msg)
        {
            if (Broadcast != null)
            {
                this.Broadcast(new BroadcastEventArgs(fromUser, msg));
            }
        }
        internal void OnUserJoin(User newUser)
        {
            if (UserJoin != null)
            {
                this.UserJoin(new UserJoinEventArgs(newUser));
            }
        }
        internal void OnUserLeave(User user)
        {
            if (UserLeave != null)
            {
                this.UserLeave(new UserLeaveEventArgs(user));
            }
        }
        internal void OnUserWhisper(User fromUser, User toUser, Message msg)
        {
            if (UserWhisper != null)
            {
                this.UserWhisper(new UserWhisperEventArgs(fromUser, toUser, msg));
            }
        }
        protected override void OnSessionClosed(UserSession session, CloseReason reason)
        {
            base.OnSessionClosed(session, reason);
        }
    }
    public class UserJoinEventArgs : EventArgs
    {
        public UserJoinEventArgs(User user)
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
    public class BroadcastEventArgs : EventArgs
    {
        public BroadcastEventArgs(User user, Message message)
        {
            User = user;
            Message = message;
        }
        public User User { get; private set; }
        public Message Message { get; private set; }
    }
    public class UserWhisperEventArgs : EventArgs
    {
        public UserWhisperEventArgs(User fromUser, User toUser, Message msg)
        {
            FromUser = fromUser;
            Message = msg;
            ToUser = toUser;
        }
        public User FromUser { get; private set; }
        public User ToUser { get; private set; }
        public Message Message { get; private set; }
    }
    public class SendToServerEventArgs : EventArgs
    {
        public SendToServerEventArgs(User user, Message message)
        {
            User = user;
            Message = message;
        }
        public User User { get; private set; }
        public Message Message { get; private set; }
    }
}
