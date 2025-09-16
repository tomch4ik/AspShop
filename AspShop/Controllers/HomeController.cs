using System.Diagnostics;
using System.Security.Claims;
using AspShop.Data;
using AspShop.Models;
using AspShop.Models.Home;
using AspShop.Services.Kdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKdfService _kdfService;
        private readonly DataContext _dataContext;
        private readonly DataAccessor _dataAccessor;

        public HomeController(ILogger<HomeController> logger, IKdfService kdfService, DataContext dataContext, DataAccessor dataAccessor)
        {
            _logger = logger;
            _kdfService = kdfService;
            _dataContext = dataContext;
            _dataAccessor = dataAccessor;
        }

        //2744FC45FF2F7CACD2EB
        public IActionResult Index()
        {
            // 2744FC45FF2F7CACD2EB
            // ViewData["dk"] = _kdfService.Dk("Admin", "4506C7");

            HomeIndexViewModel model = new()
            {
                //ProductGroups = _dataContext
                //    .ProductGroups
                //    .Where(g => g.DeletedAt == null)
                //    .AsEnumerable()
                ProductGroups = _dataAccessor.GetProductGroups()
            };

            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Category(String id)
        {
            HomeCategoryViewModel model = new()
            {
                ProductGroup = _dataContext
                    .ProductGroups
                    .Include(g => g.Products)
                    .AsNoTracking()
                    .FirstOrDefault(g => g.Slug == id && g.DeletedAt == null)
            };
            return View(model);
        }

        public IActionResult Admin()
        {
            bool isAdmin = HttpContext.User.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.Role)
                        ?.Value == "Admin";
            if (isAdmin)
            {
                HomeAdminViewModel model = new()
                {
                    ProductGroups = _dataContext
                        .ProductGroups
                        .Where(g => g.DeletedAt == null)
                        .AsEnumerable()
                };
                return View(model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
