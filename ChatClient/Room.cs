using System.Collections.Generic;
using ChatMessages;

namespace ChatClient
{
    public class Room
    {
        public Room(string id, string name)
        {
            Messages = new List<Message>();
            ID = id;
            Name = name;
        }

        public string ID { get; }
        public string Name { get; }
        public List<Message> Messages { get; }
    }
}
