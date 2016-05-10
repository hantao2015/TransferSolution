using System;
namespace UserCommon
{
    [Serializable]
    public class Message
    {
        public DateTime MsgTime { get; set; }
        public string Content { get; set; }
        public Message(string content)
        {
            Content = content;
            MsgTime = DateTime.Now;
        }
    }
}
