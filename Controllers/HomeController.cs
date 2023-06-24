using ChatBotCoachWebsite.Helpers;
using ChatBotCoachWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChatBotCoachWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BuildPineconeIndex _buildKnowledgeBase;
        private readonly QueryPineconeIndex _queryPineconeIndex;

        public HomeController(ILogger<HomeController> logger, BuildPineconeIndex buildKnowledgeBase, QueryPineconeIndex queryPineconeIndex)
        {
            _logger = logger;
            _buildKnowledgeBase = buildKnowledgeBase;
            _queryPineconeIndex = queryPineconeIndex;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> ChatAsync()
        {
            //await _buildKnowledgeBase.UpsertPineconeIndexAsync();
            //TODO: omit this when providing the user question as input somewhere else (probably in the controller)
            //for now, user question is hardcoded to be used as an example question
            string userQuestion = "What can I learn about target priority?";
            uint topIndexResults = 5;
            await _queryPineconeIndex.GetRelevantContextAsync(userQuestion, topIndexResults);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}