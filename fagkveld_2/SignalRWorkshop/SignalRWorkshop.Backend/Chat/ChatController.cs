using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalRWorkshop.Backend.Chat
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
        public void SendMessage([FromServices] IHubContext<ChatHub, IChatClient> chatHub, string message)
        {
            chatHub.Clients.All.ReceiveMessage(message);
        }
    }
}
