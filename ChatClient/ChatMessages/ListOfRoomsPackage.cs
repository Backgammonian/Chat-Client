using System.Collections.Generic;
using Newtonsoft.Json;
using Shared;

namespace ChatMessages
{
    public class ListOfRoomsPackage : IPackage
    {
        private readonly List<(string, string)> _listOfRooms;

        public ListOfRoomsPackage(List<(string, string)> listOfRooms)
        {
            Type = PackageTypes.ListOfRooms;
            _listOfRooms = listOfRooms;
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(JsonConvert.SerializeObject(_listOfRooms));

            return writer.Get();
        }
    }
}
