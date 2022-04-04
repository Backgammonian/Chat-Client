using System.Collections.Generic;
using Newtonsoft.Json;
using Shared;

namespace ChatMessages
{
    public class ResponseAllMessagesInRoomPackage : IPackage
    {
        private readonly (string, List<Message>) _listOfMessagesInRoom;

        public ResponseAllMessagesInRoomPackage(string roomID, List<Message> messages)
        {
            Type = PackageTypes.ResponseAllMessagesInRoom;
            _listOfMessagesInRoom = (roomID, messages);
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(JsonConvert.SerializeObject(_listOfMessagesInRoom));

            return writer.Get();
        }
    }
}
