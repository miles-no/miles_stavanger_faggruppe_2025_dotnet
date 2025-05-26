using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GrafanaWorkshop.Backend.Chat
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub, IChatClient> chatHub;

        public ChatController(IHubContext<ChatHub, IChatClient> chatHub)
        {
            this.chatHub = chatHub;
        }

        [HttpPost]
        public void SendMessage(string message)
        {
            chatHub.Clients.All.ReceiveMessage(message);
        }
    }
}
