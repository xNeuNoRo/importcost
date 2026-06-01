using ImportCostPro.Application.DTOs.ImportExpense.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.ImportExpenseViewModels;
using ImportCostPro.Application.ViewModels.ImportOrderViewModels;
using ImportCostPro.Persistence.Enums;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.WebApp.Controllers
{
    public class ImportExpenseController : Controller
    {
        private readonly ImportExpenseService _importExpenseService;
        private readonly ImportOrderService _importOrderService;
        private readonly CurrencyService _currencyService;

        public ImportExpenseController(
            ImportExpenseService importExpenseService,
            ImportOrderService importOrderService,
            CurrencyService currencyService)
        {
            _importExpenseService = importExpenseService;
            _importOrderService = importOrderService;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Manage(int orderId, string? source = null)
        {
            var order = await _importOrderService.GetByIdAsync(orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "La orden solicitada no existe.";
                return RedirectToAction("Index", "ImportOrder");
            }

            var items = await _importExpenseService.GetExpensesByOrderIdAsync(orderId);
            ViewBag.Order = order.Adapt<ImportOrderViewModel>();
            ViewBag.Source = source;
            ViewData["Title"] = $"Gastos - Orden {order.OrderNumber}";
            
            return View(items.Adapt<IEnumerable<ImportExpenseViewModel>>());
        }

        public async Task<IActionResult> Add(int orderId, string? source = null)
        {
            var order = await _importOrderService.GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Open)
            {
                TempData["ErrorMessage"] = "La orden no permite registrar gastos.";
                return RedirectToAction(nameof(Manage), new { orderId, source });
            }

            await PopulateCurrenciesAsync();
            ViewBag.OrderId = orderId;
            ViewBag.OrderNumber = order.OrderNumber;
            ViewBag.Source = source;
            return View(new CreateImportExpenseViewModel { ImportOrderId = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateImportExpenseViewModel viewModel, string? source = null)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCurrenciesAsync();
                ViewBag.OrderId = viewModel.ImportOrderId;
                ViewBag.Source = source;
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateImportExpenseRequest>();
                await _importExpenseService.CreateAsync(request);
                TempData["SuccessMessage"] = "Gasto registrado correctamente.";
                return RedirectToAction(nameof(Manage), new { orderId = viewModel.ImportOrderId, source });
            }
            catch (ValidationBusinessException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error inesperado al registrar el gasto.");
            }

            await PopulateCurrenciesAsync();
            ViewBag.OrderId = viewModel.ImportOrderId;
            ViewBag.Source = source;
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id, string? source = null)
        {
            var item = await _importExpenseService.GetByIdAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "El registro de gasto solicitado no existe.";
                return RedirectToAction("Index", "ImportOrder");
            }

            var order = await _importOrderService.GetByIdAsync(item.ImportOrderId);
            if (order == null || order.Status != OrderStatus.Open)
            {
                TempData["ErrorMessage"] = "Esta orden está bloqueada para modificaciones.";
                return RedirectToAction(nameof(Manage), new { orderId = item.ImportOrderId, source });
            }

            await PopulateCurrenciesAsync(item.CurrencyId);
            ViewBag.OrderNumber = order.OrderNumber;
            ViewBag.OrderId = item.ImportOrderId;
            ViewBag.Source = source;

            return View(item.Adapt<UpdateImportExpenseViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateImportExpenseViewModel viewModel, string? source = null)
        {
            var item = await _importExpenseService.GetByIdAsync(viewModel.Id);
            int orderId = item?.ImportOrderId ?? 0;

            if (!ModelState.IsValid)
            {
                await PopulateCurrenciesAsync(item?.CurrencyId ?? 0);
                ViewBag.OrderId = orderId;
                ViewBag.Source = source;
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateImportExpenseRequest>();
                await _importExpenseService.UpdateAsync(request);
                TempData["SuccessMessage"] = "Gasto actualizado correctamente.";
                return RedirectToAction(nameof(Manage), new { orderId, source });
            }
            catch (ValidationBusinessException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar el gasto.");
            }

            await PopulateCurrenciesAsync(item?.CurrencyId ?? 0);
            ViewBag.OrderId = orderId;
            ViewBag.Source = source;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? source = null)
        {
            var item = await _importExpenseService.GetByIdAsync(id);
            if (item == null) return RedirectToAction("Index", "ImportOrder");

            int orderId = item.ImportOrderId;
            try
            {
                await _importExpenseService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Gasto eliminado correctamente.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al intentar eliminar el gasto.";
            }

            return RedirectToAction(nameof(Manage), new { orderId, source });
        }

        private async Task PopulateCurrenciesAsync(int? currentCurrencyId = null)
        {
            var currencies = await _currencyService.GetAllAsync();
            
            var items = currencies
                .Where(c => c.IsActive || c.Id == currentCurrencyId)
                .Select(c => new {
                    c.Id,
                    DisplayName = c.IsActive ? c.Name : $"{c.Name} (Inactivo)"
                });

            ViewBag.Currencies = new SelectList(items, "Id", "DisplayName");
        }
    }
}
