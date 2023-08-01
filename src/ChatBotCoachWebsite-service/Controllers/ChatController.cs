using ChatBotCoachWebsite.Areas.Identity.Data;
using ChatBotCoachWebsite.Data;
using ChatBotCoachWebsite.Helpers;
using ChatBotCoachWebsite.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatBotCoachWebsite.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private UserManager<ChatBotCoachWebsiteUser> _userManager;
        private SignInManager<ChatBotCoachWebsiteUser> _signInManager;
        public User _user;

        public ChatController(IHubContext<ChatHub> hubContext, UserManager<ChatBotCoachWebsiteUser> userManager,
                              SignInManager<ChatBotCoachWebsiteUser> signInManager, User user)
        {
            _hubContext = hubContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _user = user;
        }

        public async Task<IActionResult> ChatAsync()
        {
            bool isSignedIn = _user.IsSignedIn(User);
            string firstName = await _user.GetFirstNameAsync(User);
            ViewData["FirstName"] = firstName;
            ViewData["IsSignedIn"] = isSignedIn;
            return View();
        }
    }
}
