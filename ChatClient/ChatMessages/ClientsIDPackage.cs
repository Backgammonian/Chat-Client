using Newtonsoft.Json;
using Shared;

namespace ChatMessages
{
    public class ClientsIDPackage : IPackage
    {
        private readonly string _clientID;

        public ClientsIDPackage(string clientID)
        {
            Type = PackageTypes.ClientsID;
            _clientID = clientID;
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(JsonConvert.SerializeObject(_clientID));

            return writer.Get();
        }
    }
}
