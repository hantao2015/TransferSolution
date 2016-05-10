using System;
using UserCommon;

namespace UserClientLib
{
    public interface IChatEngine
    {
        void UserChat(Message message);
    }
    public interface IChatEvents
    {
        void OnUserChat(Message message);
    }
}
