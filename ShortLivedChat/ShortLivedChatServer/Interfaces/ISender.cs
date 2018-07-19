namespace ShortLivedChatServer.Interfaces
{
    public interface ISender
    {
        //send message from server
        void SendMessageFromServer(string message);

        //notify all clients that the chat channel is closing
        void CloseChatChannel();
    }
}