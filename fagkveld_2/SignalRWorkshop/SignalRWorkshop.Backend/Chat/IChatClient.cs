namespace SignalRWorkshop.Backend.Chat
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
    }
}
