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

        public HomeController(ILogger<HomeController> logger, BuildPineconeIndex buildKnowledgeBase)
        {
            _logger = logger;
            _buildKnowledgeBase = buildKnowledgeBase;
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
           // await _buildKnowledgeBase.CreateVectorsAsync();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}