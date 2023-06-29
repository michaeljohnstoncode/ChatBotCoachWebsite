using ChatBotCoachWebsite.Models;

namespace ChatBotCoachWebsite.Helpers.Services
{
    public interface IAiChatService
    {
        Task<UserMessageModel> GetAiResponse(string user, string message);
    }

    //This class needs to be supplied with the whole chat conversation as context, so the ai can build upon the conversation
    public class AiChatService : IAiChatService
    {
        private QueryPineconeIndex _queryPineconeIndex;

        public AiChatService(QueryPineconeIndex queryPineconeIndex)
        {
            _queryPineconeIndex = queryPineconeIndex;
        }

        public async Task<UserMessageModel> GetAiResponse(string user, string message)
        {
            UserMessageModel aiResponse = await _queryPineconeIndex.AiCompletionResponse(user, message);
            return aiResponse;
        }
    }

}
