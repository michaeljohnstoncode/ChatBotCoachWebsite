using Microsoft.IdentityModel.Tokens;

namespace ChatBotCoachWebsite.Models
{
    public class Intents
    {
        //Each intent will branch off into more intents to narrow down 


        public string AbilityUsage { get; set; }
        public string UltUsage { get; set; }
        public string Map { get; set; }
        public string Character { get; set; }
        public string Role { get; set; }
        public string Aim { get; set; }
        public string Teamwork { get; set; }
        public string Deaths { get; set; }
        public string Positioning { get; set; }
        public string PlayerAwareness { get; set; }
    }
}
