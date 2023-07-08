using ChatBotCoachWebsite.Models;

namespace ChatBotCoachWebsite.Helpers.Services
{
    public interface IAiChatService
    {
        Task<MessageAndConversation> GetAiResponse(string user, string message, List<OpenAI_API.Chat.ChatMessage> chatConversation);
    }

    //the intention is to conserve tokens so the chat conversation should have each message summarized.
    //this is used to build the conversation as context for the AI
    public class AiChatService : IAiChatService
    {
        private QueryPineconeIndex _queryPineconeIndex;
        private ISummarizeChatService _summarizeMessage;

        public AiChatService(QueryPineconeIndex queryPineconeIndex, ISummarizeChatService summarizeMessage)
        {
            _queryPineconeIndex = queryPineconeIndex;
            _summarizeMessage = summarizeMessage;
        }

        /*
        ***TODO***

           This following method adds the current conversation to a chatConversation variable declared in ChatHub.cs
           This doesn't seem like a good idea as it doesn't follow a flow of operations.

           What this method does is get the AI's response to the User's message and returns it,
           but at the same time it adds to the chatConversation variable out of scope in the ChatHub.cs


         */
        public async Task<MessageAndConversation> GetAiResponse(string user, string message, List<OpenAI_API.Chat.ChatMessage> chatConversation)
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

            //get ai response to user's message by querying Pinecone index
            UserMessage aiResponse = await _queryPineconeIndex.AiCompletionResponse(chatConversation);

            //summarize ai response for token conservation (for conversation context)
            UserMessage summarizedAiResponse = await _summarizeMessage.SummarizeMessage(aiResponse);

            //add summarized ai response to chatConversation
            OpenAI_API.Chat.ChatMessage aiSummarizedResponse = new()
            {
                Name = summarizedAiResponse.User,
                Role = OpenAI_API.Chat.ChatMessageRole.Assistant,
                Content = summarizedAiResponse.Message
            };
            chatConversation.Add(aiSummarizedResponse);

            //return ai response and chat conversation context
            MessageAndConversation aiResponseAndConversation = new()
            {
                MessageModel = aiResponse,
                ChatConversation = chatConversation
            };

            return aiResponseAndConversation;
        }
    }

}
