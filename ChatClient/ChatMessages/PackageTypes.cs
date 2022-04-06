namespace ChatMessages
{
    public enum PackageTypes : byte
    {
        ClientHello = 10, //client -> server
        ClientsNewNickname, //client -> server
        ListOfRooms, //server -> client
        RequestAllMessagesInRoom, //client -> server
        ResponseAllMessagesInRoom, //server -> client
        MessageToRoom, //client -> server
        NewMessageInRoom, //server -> client
        ClientDisconnect, //client -> server
        ClientsListUpdated, //server -> client
    }
}
