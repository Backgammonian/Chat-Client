using System.Collections.Generic;
using Newtonsoft.Json;
using Shared;

namespace ChatMessages
{
    public class ClientsListUpdatedPackage : IPackage
    {
        //nickname, ID, address
        private readonly List<(string, string, string)> _listOfClients;

        public ClientsListUpdatedPackage(List<(string, string, string)> listOfClients)
        {
            Type = PackageTypes.ClientsListUpdated;
            _listOfClients = listOfClients;
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(JsonConvert.SerializeObject(_listOfClients));

            return writer.Get();
        }
    }
}
