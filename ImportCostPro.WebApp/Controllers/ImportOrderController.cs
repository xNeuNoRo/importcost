using ImportCostPro.Application.DTOs.ImportOrder.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.ImportOrderViewModels;
using ImportCostPro.Persistence.Enums;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.WebApp.Controllers
{
    public class ImportOrderController : Controller
    {
        private readonly ImportOrderService _importOrderService;
        private readonly ImporterService _importerService;
        private readonly SupplierService _supplierService;
        private readonly CountryService _countryService;
        private readonly CurrencyService _currencyService;

        public ImportOrderController(
            ImportOrderService importOrderService,
            ImporterService importerService,
            SupplierService supplierService,
            CountryService countryService,
            CurrencyService currencyService)
        {
            _importOrderService = importOrderService;
            _importerService = importerService;
            _supplierService = supplierService;
            _countryService = countryService;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _importOrderService.GetAllAsync();
            var viewModel = orders.Adapt<IEnumerable<ImportOrderViewModel>>();
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropDownsAsync();
            return View(new CreateImportOrderViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateImportOrderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync();
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateImportOrderRequest>();
                await _importOrderService.CreateAsync(request);
                TempData["SuccessMessage"] = "Orden de importación creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationBusinessException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al crear la orden.";
            }

            await PopulateDropDownsAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _importOrderService.GetByIdAsync(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "La orden solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = order.Adapt<UpdateImportOrderViewModel>();
            await PopulateDropDownsAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _importOrderService.GetByIdAsync(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "La orden solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = order.Adapt<UpdateImportOrderViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateImportOrderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync();
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateImportOrderRequest>();
                await _importOrderService.UpdateAsync(request);
                TempData["SuccessMessage"] = "Orden de importación actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationBusinessException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al actualizar la orden.";
            }

            await PopulateDropDownsAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _importOrderService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Orden eliminada con éxito.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar la orden.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, OrderStatus newStatus)
        {
            try
            {
                await _importOrderService.ChangeStatusAsync(id, newStatus);
                TempData["SuccessMessage"] = $"Estado de la orden actualizado a {newStatus}.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error inesperado al cambiar el estado de la orden.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropDownsAsync()
        {
            var importers = await _importerService.GetAllAsync();
            var suppliers = await _supplierService.GetAllAsync();
            var countries = await _countryService.GetAllAsync();
            var currencies = await _currencyService.GetAllAsync();

            ViewBag.Importers = new SelectList(importers.Where(x => x.IsActive), "Id", "LegalName");
            ViewBag.Suppliers = new SelectList(suppliers.Where(x => x.IsActive), "Id", "Name");
            ViewBag.Countries = new SelectList(countries.Where(x => x.IsActive), "Id", "Name");
            ViewBag.Currencies = new SelectList(currencies.Where(x => x.IsActive), "Id", "IsoCode");
        }
    }
}
