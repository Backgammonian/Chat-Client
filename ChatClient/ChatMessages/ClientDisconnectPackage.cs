using Shared;

namespace ChatMessages
{
    public class ClientDisconnectPackage : IPackage
    {
        public ClientDisconnectPackage()
        {
            Type = PackageTypes.ClientDisconnect;
        }

        public PackageTypes Type { get; }

        public byte[] GetByteArray()
        {
            var writer = new SimpleWriter();

            writer.Put((byte)Type);
            writer.Put(string.Empty);

            return writer.Get();
        }
    }
}
