using ChatBotCoachWebsite.Helpers.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task SendMessage(string user, string message)
    {
        //save messages to provide access to chat data
        await _chatService.SaveMessage(user, message);
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
