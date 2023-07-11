using OpenAI_API;

namespace ChatBotCoach_logic.Clients.Contracts
{
    public interface IOpenAIClient
    {
        public Task<OpenAIAPI> GetOpenAI(CancellationToken cancellationToken);
    }
}
