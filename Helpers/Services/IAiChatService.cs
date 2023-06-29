using ChatBotCoachWebsite.Models;

namespace ChatBotCoachWebsite.Helpers.Services
{
    public interface IAiChatService
    {
        Task<MessageAndConversationModel> GetAiResponse(string user, string message, List<OpenAI_API.Chat.ChatMessage> chatConversation);
    }

    //This class needs to be supplied with the whole chat conversation as context, so the ai can build upon the conversation

    //possibly to conserve tokens, chat conversation should have each message summarized, to build a context of what was discussed in the conversation
    public class AiChatService : IAiChatService
    {
        private QueryPineconeIndex _queryPineconeIndex;
        private ISummarizeChatService _summarizeMessage;

        public AiChatService(QueryPineconeIndex queryPineconeIndex, ISummarizeChatService summarizeMessage)
        {
            _queryPineconeIndex = queryPineconeIndex;
            _summarizeMessage = summarizeMessage;
        }

        public async Task<MessageAndConversationModel> GetAiResponse(string user, string message, List<OpenAI_API.Chat.ChatMessage> chatConversation)
        {
            //build the conversation context, this affects how the ai will respond
            //this is what the user {user} asked: {message}
            //you(ai) responded with: {aiResponse}

            //add user+message to chatConversation
            OpenAI_API.Chat.ChatMessage userMsg = new()
            {
                Name = user,
                Role = OpenAI_API.Chat.ChatMessageRole.User,
                Content = message
            };
            chatConversation.Add(userMsg);

            //get ai response to user's message
            MessageModel aiResponse = await _queryPineconeIndex.AiCompletionResponse(chatConversation);

            //summarize ai response for token conservation (for conversation context)
            MessageModel summarizedAiResponse = await _summarizeMessage.SummarizeMessage(aiResponse);
            //add summarized ai response to chatConversation
            OpenAI_API.Chat.ChatMessage aiSummarizedResponse = new()
            {
                Name = summarizedAiResponse.User,
                Role = OpenAI_API.Chat.ChatMessageRole.Assistant,
                Content = summarizedAiResponse.Message
            };
            chatConversation.Add(aiSummarizedResponse);

            //return ai response and chat conversation context
            MessageAndConversationModel aiResponseAndConversation = new()
            {
                MessageModel = aiResponse,
                ChatConversation = chatConversation
            };

            return aiResponseAndConversation;
        }
    }

}
