using ChatBotCoachWebsite.Helpers;
using ChatBotCoachWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChatBotCoachWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GetPineconeIndex _buildKnowledgeBase;
        private readonly QueryPineconeIndex _queryPineconeIndex;

        public HomeController(ILogger<HomeController> logger, GetPineconeIndex buildKnowledgeBase, QueryPineconeIndex queryPineconeIndex)
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}