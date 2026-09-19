using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LegacyHealthcareFHIR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISignalRNotifier _signalRNotifier;

        public HomeController(ISignalRNotifier signalRNotifier)
        {
            _signalRNotifier = signalRNotifier;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TestNotification()
        {
            await _signalRNotifier.SendAsync(
                "TestNotification",
                new
                {
                    Message = "SignalR is working!",
                    Time = DateTime.UtcNow
                });

            return Ok();
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
