using ChatBotCoachWebsite.Helpers.Services;
using ChatBotCoachWebsite.Models;

namespace ChatBotCoachWebsite.Helpers
{
    public interface ISummarizeChatService
    {
        Task<UserMessage> SummarizeMessage(UserMessage msg);
    }

    public class SummarizeMessageService : ISummarizeChatService
    {
        private IOpenAIService _openAiService;
        private Prompts _prompts;
        public SummarizeMessageService(IOpenAIService openAIService, Prompts prompts)
        {
            _openAiService = openAIService;
            _prompts = prompts;
        }

        public async Task<UserMessage> SummarizeMessage(UserMessage msg)
        {
            //get openai instance
            var openAi = _openAiService.GetOpenAI();

            //create message with prompt
            string promptedMsg = _prompts.SummaryPrompt() + msg.Message;

            //format promptedMsg to be inputted to openai chat completion
            OpenAI_API.Chat.ChatMessage promptedChatMsg = new()
            {
                Content = promptedMsg,
                Role = OpenAI_API.Chat.ChatMessageRole.User,
            };
            List<OpenAI_API.Chat.ChatMessage> openAiChatMsg = new();
            openAiChatMsg.Add(promptedChatMsg);

            //get summary of message
            var result = await openAi.Chat.CreateChatCompletionAsync(openAiChatMsg, model: OpenAI_API.Models.Model.ChatGPTTurbo);

            UserMessage msgSummary = new()
            {
                Message = result.Choices[0].Message.Content,
                User = msg.User
            };
            return msgSummary;
        }
    }
}
