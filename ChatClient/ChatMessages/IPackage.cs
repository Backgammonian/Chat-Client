namespace ChatMessages
{
    public interface IPackage
    {
        byte[] GetByteArray();
        PackageTypes Type { get; }
    }
}
