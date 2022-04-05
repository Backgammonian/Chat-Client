namespace ChatClient
{
    public class ClientOfServer
    {
        public ClientOfServer(string nickname, string id, string address)
        {
            Nickname = nickname;
            ID = id;
            Address = address;
        }

        public string Nickname { get; }
        public string ID { get; }
        public string Address { get; }
    }
}
