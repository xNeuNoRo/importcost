using ImportCostPro.Application.DTOs.LandedCost.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.LandedCostViewModels;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.WebApp.Controllers
{
    public class LandedCostController : Controller
    {
        private readonly LandedCostService _landedCostService;
        private readonly ImportOrderService _importOrderService;
        private readonly ICalculationResultRepository _calculationResultRepository;

        public LandedCostController(
            LandedCostService landedCostService, 
            ImportOrderService importOrderService,
            ICalculationResultRepository calculationResultRepository)
        {
            _landedCostService = landedCostService;
            _importOrderService = importOrderService;
            _calculationResultRepository = calculationResultRepository;
        }

        public async Task<IActionResult> Index()
        {
            var openOrders = await _importOrderService.GetAllAsync();
            var openOrdersList = openOrders.Where(o => o.Status == Persistence.Enums.OrderStatus.Open);
            
            ViewBag.Orders = new SelectList(openOrdersList, "Id", "OrderNumber");
            ViewData["Title"] = "Liquidación de Costos";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(int orderId)
        {
            if (orderId <= 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar una orden válida.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var request = new ProcessCalculationRequest { ImportOrderId = orderId };
                var response = await _landedCostService.ProcessCalculationAsync(request);
                
                TempData["SuccessMessage"] = "Liquidación de Costos procesada y guardada correctamente.";
                return RedirectToAction(nameof(Details), new { orderId });
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al procesar el cálculo.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int orderId)
        {
            var result = await _calculationResultRepository.GetLatestCalculatedResultWithDetailsAsync(orderId);
            if (result == null)
            {
                TempData["ErrorMessage"] = "No se encontró un cálculo oficial para esta orden.";
                return RedirectToAction("Index", "ImportOrder");
            }

            var viewModel = result.Adapt<LandedCostCalculationViewModel>();
            ViewData["Title"] = $"Resultado de Liquidación - Orden {viewModel.OrderNumber}";
            return View(viewModel);
        }
    }
}
