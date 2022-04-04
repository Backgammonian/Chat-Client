using Newtonsoft.Json;
using Shared;

namespace ChatMessages
{
    public class RequestAllMessagesInRoomPackage : IPackage
    {
        private readonly string _roomID;

        public RequestAllMessagesInRoomPackage(string roomID)
        {
            Type = PackageTypes.RequestAllMessagesInRoom;
            _roomID = roomID;
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(JsonConvert.SerializeObject(_roomID));

            return writer.Get();
        }
    }
}
