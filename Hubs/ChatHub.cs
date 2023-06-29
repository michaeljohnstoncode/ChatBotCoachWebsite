using ChatBotCoachWebsite.Helpers.Services;
using ChatBotCoachWebsite.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private IAiChatService _aiChatService;
    public static List<OpenAI_API.Chat.ChatMessage> _chatConversation = new();

    public ChatHub(IAiChatService aiChatService)
    {
        _aiChatService = aiChatService;
    }

    //TODO: Left off at creating the chat conversation context. It was successful but there are questions to think about:
    //should I be querying pinecone index every time? (probably?)
    //is this chat conversation context going to be taking context from all users? currently, I think every input gets put into the chatConversation.
    //if conversation context IS shared between all users, then how would I seperate it for each user? can it be stored in memory in their browser?

    public async Task SendMessage(string user, string message)
    {
        //receive the user's message and send it to all parties (which is just user and bot's chat log, so user is the only party)
        await Clients.All.SendAsync("ReceiveMessage", user, message);

        //get the ai's response to the user's message
        MessageAndConversationModel aiResponse = await _aiChatService.GetAiResponse(user, message, _chatConversation);
        //update _chatConversation with new responses
        _chatConversation = aiResponse.ChatConversation;
        //send ai response to chat
        await Clients.All.SendAsync("ReceiveMessage", aiResponse.MessageModel.User, aiResponse.MessageModel.Message);

    }
}
