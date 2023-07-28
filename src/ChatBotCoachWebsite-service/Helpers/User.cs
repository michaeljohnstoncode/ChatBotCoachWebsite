using ChatBotCoachWebsite.Areas.Identity.Data;
using ChatBotCoachWebsite.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatBotCoachWebsite.Helpers
{
    public class User
    {
        private ChatBotCoachWebsiteContext _context;
        private UserManager<ChatBotCoachWebsiteUser> _userManager;
        private SignInManager<ChatBotCoachWebsiteUser> _signInManager;

        public User(ChatBotCoachWebsiteContext context, UserManager<ChatBotCoachWebsiteUser> userManager, SignInManager<ChatBotCoachWebsiteUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<string> GetFirstNameAsync(System.Security.Claims.ClaimsPrincipal User)
        {
            var user = await _userManager.GetUserAsync(User);
            var email = user.Email;
          //  string email = "mike@gmail.com";
            string firstName = _context.Users.FirstOrDefault(x => x.Email == email).FirstName;
            return firstName;
        }

        public bool IsSignedIn(System.Security.Claims.ClaimsPrincipal User)
        {
            bool isSignedIn = _signInManager.IsSignedIn(User);
            return isSignedIn;
        }
    }
}
