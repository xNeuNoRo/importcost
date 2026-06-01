using ImportCostPro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DashboardService _dashboardService;

        public HomeController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _dashboardService.GetStatisticsAsync();
            return View(viewModel);
        }

        public IActionResult Credits()
        {
            ViewData["Title"] = "Créditos del Proyecto";
            return View();
        }

        public IActionResult Catalogues()
        {
            ViewData["Title"] = "Gestión de Catálogos";
            ViewData["Breadcrumbs"] = new List<(string Text, string? Action, string? Controller)>
            {
                ("Catálogos", null, null)
            };
            return View();
        }
    }
}
