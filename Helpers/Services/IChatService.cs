﻿using ChatBotCoachWebsite.Models;

namespace ChatBotCoachWebsite.Helpers.Services
{
    public interface IChatService
    {
        Task SaveMessage(string user, string message);
    }

    public class ChatService : IChatService
    {
        // Your database context or another service that communicates with your database goes here

        public async Task SaveMessage(string user, string message)
        {
          /*  UserMessageModel userMessage = new()
            {
                User = user,
                Message = message
            };
          */
        }
    }

}
