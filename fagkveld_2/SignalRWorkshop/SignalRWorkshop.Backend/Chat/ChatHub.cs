using Microsoft.AspNetCore.SignalR;

namespace SignalRWorkshop.Backend.Chat
{
    public class ChatHub : Hub<IChatClient>
    {
        public override async Task OnConnectedAsync()
        {
            await SendMessage("A new user has joined the chat");
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string message)
            => await Clients.All.ReceiveMessage(message);
    }
}
