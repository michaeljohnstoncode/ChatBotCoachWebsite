﻿using ChatBotCoach_config.Contracts;
using ChatBotCoach_logic.Clients;

namespace ChatBotCoach_config
{
    public class OpenAIConfiguration : IClientConfiguration<OpenAIClient>
    {
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Endpoint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
