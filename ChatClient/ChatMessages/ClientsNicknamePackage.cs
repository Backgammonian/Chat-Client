using Newtonsoft.Json;
using Shared;

namespace ChatMessages
{
    public class ClientsNicknamePackage : IPackage
    {
        private readonly (string, string) _clientNickname;

        public ClientsNicknamePackage(string clientID, string clientNickname)
        {
            Type = PackageTypes.ClientsNickname;
            _clientNickname = (clientID, clientNickname);
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(JsonConvert.SerializeObject(_clientNickname));

            return writer.Get();
        }
    }
}
