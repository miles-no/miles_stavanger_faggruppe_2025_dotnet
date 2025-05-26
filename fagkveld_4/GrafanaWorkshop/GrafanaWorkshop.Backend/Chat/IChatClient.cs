namespace GrafanaWorkshop.Backend.Chat
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
    }
}
