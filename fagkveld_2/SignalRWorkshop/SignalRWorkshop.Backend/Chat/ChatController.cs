using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalRWorkshop.Backend.Chat
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public void SendMessage([FromServices] IHubContext<ChatHub, IChatClient> chatHub, string message)
        {
            chatHub.Clients.All.ReceiveMessage(message);
        }
    }
}
