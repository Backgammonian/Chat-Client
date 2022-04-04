using Newtonsoft.Json;
using Shared;

namespace ChatMessages
{
    public class NewMessageInRoomPackage : IPackage
    {
        private readonly (Message, string) _newMessageInRoom;

        public NewMessageInRoomPackage(Message message, string roomID)
        {
            Type = PackageTypes.NewMessageInRoom;
            _newMessageInRoom = (message, roomID);
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(JsonConvert.SerializeObject(_newMessageInRoom));

            return writer.Get();
        }
    }
}
