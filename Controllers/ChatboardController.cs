using Microsoft.Extensions.Configuration; // Fixes IConfiguration error
using Mscc.GenerativeAI;           // Fixes GoogleAI / GenerativeModel namespace

using PhraseBookk.Models;
using Microsoft.AspNetCore.Mvc;

namespace PhraseBookk.Controllers
{
    public class ChatboardController : Controller
    {
        private readonly Chatboard _chatboard;

        // ASP.NET injects your Chatboard service
        public ChatboardController(Chatboard chatboard)
        {
            _chatboard = chatboard;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ask(string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(userQuestion))
            {
                return Json(new { response = "Please enter a question." });
            }
            try
            {
                string botResponse = await _chatboard.AskAsync(userQuestion);
                return Json(new { response = botResponse });


            }
            catch (Exception ex)
            {
                // Helpful for debugging API key or model errors
                return Json(new { response = $"Error: {ex.Message}" });
            }
        }
    }
}
