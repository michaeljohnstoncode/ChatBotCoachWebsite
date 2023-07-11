using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotCoach_data.models
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public DateTime CreatedDate { get; set; }
        public TokenModel()
        {
            CreatedDate = DateTime.Now;
        }
        public static TokenModel FromText(string key)
        {
            return new TokenModel()
            {
                AccessToken = key,
                ExpiresIn = 0,
                TokenType = "Bearer",
                Scope = "engine: davinci, gpt-4, gpt-3.5, gpt-turbo, ada",
            };
        }
    }
}
