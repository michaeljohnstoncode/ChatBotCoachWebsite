using ChatBotCoachWebsite.Helpers;
using ChatBotCoachWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatBotCoachWebsite.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
/*
        public IActionResult Test()
        {
            return View();
        }
*/
        public async Task<IActionResult> ChatAsync()
        {

            return View();

        }

    }
}
