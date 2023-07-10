using OpenAI_API;

namespace ChatBotCoachWebsite.Helpers.Services
{
    public interface IOpenAIService
    {
        OpenAIAPI GetOpenAI();
    }

    public class OpenAIService : IOpenAIService
    {
        private IKeyProvider _keyProvider;

        public OpenAIService(IKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }

        public OpenAIAPI GetOpenAI()
        {
            //get openAI api key
            var openAiApiKey = _keyProvider.GetKey("openaikey");
            //initialize openAi
            OpenAIAPI openAi = new OpenAIAPI(openAiApiKey);
            return openAi;
        }
    }
}
