
namespace UserCommon
{
    public enum UserOP
    {
        DoLogin = 1,
        DoLoginResult = 2,
        DoLogOut = 3,
        DoUserJoin = 4,
        DoUserLeave = 5,
        DoGetAllUser = 6,
        DoGetUser = 7,
        DoWhisper = 8,
        DoBroadCast = 9,
        DoSendToServer = 10,
        DoNoticeUser = 11,

        DoUserStartCreateChat = 12,
        DoNewChat = 13,
        //DoCreateChat = 13,
        DoUserChat = 14,
        DoChatLeave = 15,
        DoChatConn = 16,
    }
}
