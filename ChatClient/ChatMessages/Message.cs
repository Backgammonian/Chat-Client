using System;

namespace ChatMessages
{
    public class Message
    {
        public Message(string content, string authorsNickname, string authorsID, DateTime time)
        {
            Content = content;
            AuthorsNickname = authorsNickname;
            AuthorsID = authorsID;
            Time = time;
        }

        public string Content { get; }
        public string AuthorsNickname { get; }
        public string AuthorsID { get; }
        public DateTime Time { get; }
    }
}
