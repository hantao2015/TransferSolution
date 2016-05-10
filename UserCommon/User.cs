
using System;
namespace UserCommon
{
    [Serializable]
    public class User
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
