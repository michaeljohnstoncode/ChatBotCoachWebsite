using ChatBotCoachWebsite.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotCoachWebsite.Controllers
{
    public class ChatController : Controller
    {
        private readonly BuildPineconeIndex _buildKnowledgeBase;
        private readonly QueryPineconeIndex _queryPineconeIndex;

        public ChatController(ILogger<HomeController> logger, BuildPineconeIndex buildKnowledgeBase, QueryPineconeIndex queryPineconeIndex)
        {
            _buildKnowledgeBase = buildKnowledgeBase;
            _queryPineconeIndex = queryPineconeIndex;
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
          //  string userQuestion = "\nQuestion: What can I learn about target priority?";
          //  uint topIndexResults = 2;
          //  await _queryPineconeIndex.CreateFullAiPromptAsync(userQuestion, topIndexResults);
            return View();
        }

    }
}
