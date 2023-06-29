using ChatBotCoachWebsite.Helpers.Services;
using ChatBotCoachWebsite.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private IAiChatService _chatService;

    public ChatHub(IAiChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task SendMessage(string user, string message)
    {
        //receive the user's message and send it to all parties (which is just user and bot's chat log, so user is the only party)
        await Clients.All.SendAsync("ReceiveMessage", user, message);

        //get the ai's response to the user's message
        UserMessageModel aiResponse = await _chatService.GetAiResponse(user, message);
        //send ai response to chat
        await Clients.All.SendAsync("ReceiveMessage", aiResponse.User, aiResponse.Message);

    }
}
