using Newtonsoft.Json;
using Shared;

namespace ChatMessages
{
    public class ClientHelloPackage : IPackage
    {
        private readonly (string, string) _clientInfo;

        public ClientHelloPackage(string clientID, string clientNickname)
        {
            Type = PackageTypes.ClientHello;
            _clientInfo = (clientID, clientNickname);
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(JsonConvert.SerializeObject(_clientInfo));

            return writer.Get();
        }
    }
}
