using Newtonsoft.Json;
using Shared;

namespace ChatMessages
{
    public class MessageToRoomPackage : IPackage
    {
        private readonly (Message, string) _messageToRoom;

        public MessageToRoomPackage(Message message, string roomID)
        {
            Type = PackageTypes.MessageToRoom;
            _messageToRoom = (message, roomID);
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(JsonConvert.SerializeObject(_messageToRoom));

            return writer.Get();
        }
    }
}
