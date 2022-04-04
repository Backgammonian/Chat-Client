namespace ChatMessages
{
    public enum PackageTypes : byte
    {
        ClientsNickname = 10, //client -> server
        ListOfRooms, //server -> client
        RequestAllMessagesInRoom, //client -> server
        ResponseAllMessagesInRoom, //server -> client
        MessageToRoom, //client -> server
        NewMessageInRoom //server -> client
    }
}
