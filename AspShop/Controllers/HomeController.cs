using System.Diagnostics;
using AspShop.Models;
using AspShop.Services.Kdf;
using Microsoft.AspNetCore.Mvc;

namespace AspShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKdfService _kdfService;

        public HomeController(ILogger<HomeController> logger, IKdfService kdfService)
        {
            _logger = logger;
            _kdfService = kdfService;
        }

        //2744FC45FF2F7CACD2EB
        public IActionResult Index()
        {
            ViewData["KdfResult"] = _kdfService.Dk("Admin", "4506C746-8FDD-4586-9BF4-95D6933C3B4F");
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
