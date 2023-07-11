using ChatBotCoach_data.models;
using ChatBotCoach_logic.Clients.Contracts;
using OpenAI_API;

namespace ChatBotCoach_logic.Clients
{
    public class OpenAIClient : IOpenAIClient
    {
        private ITokenClaimService _tokenService;

        public OpenAIClient(ITokenClaimService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<OpenAIAPI> GetOpenAI(CancellationToken cancellationToken)
        {
            //make a request for the openai token and get openAI API Key from it
            var tokenRequest = new RequestSettingsModel("openaikey");
            var openAiApiToken = await _tokenService.GetTokenAsync(tokenRequest, cancellationToken);
            var openAiApiKey = openAiApiToken.AccessToken;
            //initialize openAi
            OpenAIAPI openAi = new OpenAIAPI(openAiApiKey);
            return openAi;
        }
    }
}
