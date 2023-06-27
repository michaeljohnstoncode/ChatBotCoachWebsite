using ChatBotCoachWebsite.Helpers;
using ChatBotCoachWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatBotCoachWebsite.Controllers
{
    public class ChatController : Controller
    {
        private readonly BuildPineconeIndex _buildKnowledgeBase;
        private readonly QueryPineconeIndex _queryPineconeIndex;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(BuildPineconeIndex buildKnowledgeBase, QueryPineconeIndex queryPineconeIndex, IHubContext<ChatHub> hubContext)
        {
            _buildKnowledgeBase = buildKnowledgeBase;
            _queryPineconeIndex = queryPineconeIndex;
            _hubContext = hubContext;
        }
/*
        public IActionResult Test()
        {
            return View();
        }
*/
        public async Task<IActionResult> ChatAsync()
        {
            //await _buildKnowledgeBase.UpsertPineconeIndexAsync();
            //TODO: omit this when providing the user question as input somewhere else (probably in the controller)
            //for now, user question is hardcoded to be used as an example question
            string userQuestion = "\nQuestion: What can I learn about target priority?";
            uint topIndexResults = 2;
          // UserMessageModel userMsg = await _queryPineconeIndex.AiCompletionResponse(userQuestion, topIndexResults);
           // await _hubContext.Clients.All.SendAsync("ReceiveMessage", userMsg.User, userMsg.Message);
            return View();

        }

    }
}
