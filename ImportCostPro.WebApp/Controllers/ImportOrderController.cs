using ImportCostPro.Application.DTOs.ImportOrder.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.ImportOrderViewModels;
using ImportCostPro.Persistence.Enums;
using ImportCostPro.Persistence.Interfaces.Repositories;
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
        private readonly ICalculationResultRepository _calculationResultRepository;

        public ImportOrderController(
            ImportOrderService importOrderService,
            ImporterService importerService,
            SupplierService supplierService,
            CountryService countryService,
            CurrencyService currencyService,
            ICalculationResultRepository calculationResultRepository)
        {
            _importOrderService = importOrderService;
            _importerService = importerService;
            _supplierService = supplierService;
            _countryService = countryService;
            _currencyService = currencyService;
            _calculationResultRepository = calculationResultRepository;
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
                var response = await _importOrderService.CreateAsync(request);
                TempData["SuccessMessage"] = "Orden de importación creada correctamente. Ahora puede agregar productos y gastos.";
                return RedirectToAction(nameof(Details), new { id = response.Id });
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
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear la orden.");
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
            await PopulateDropDownsAsync(viewModel.ImporterId, viewModel.SupplierId, viewModel.OriginCountryId, viewModel.CurrencyId);
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

            ViewBag.ImporterName = order.ImporterName;
            ViewBag.SupplierName = order.SupplierName;
            ViewBag.CountryName = order.OriginCountryName;
            ViewBag.CurrencyIsoCode = order.CurrencyIsoCode;
            ViewBag.LocalCurrencySymbol = "RD$"; 

            if (order.Status == OrderStatus.Calculated || order.Status == OrderStatus.Closed)
            {
                var calculation = await _calculationResultRepository.GetLatestCalculatedResultWithDetailsAsync(id);
                if (calculation != null)
                {
                    ViewBag.TotalImportCost = calculation.TotalImportCost;
                    ViewBag.LocalCurrencySymbol = calculation.LocalCurrencyUsed?.Symbol ?? "RD$";
                }
                else
                {
                    ViewBag.TotalImportCost = 0m;
                }
            }
            else
            {
                ViewBag.TotalImportCost = 0m;
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
                await PopulateDropDownsAsync(viewModel.ImporterId, viewModel.SupplierId, viewModel.OriginCountryId, viewModel.CurrencyId);
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
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar la orden.");
            }

            await PopulateDropDownsAsync(viewModel.ImporterId, viewModel.SupplierId, viewModel.OriginCountryId, viewModel.CurrencyId);
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
                string message = newStatus switch
                {
                    OrderStatus.Closed => "Orden finalizada y cerrada oficialmente.",
                    OrderStatus.Canceled => "La orden ha sido anulada correctamente.",
                    _ => $"El estado de la orden ha sido actualizado a {newStatus.GetDisplayName()}."
                };
                TempData["SuccessMessage"] = message;
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

        private async Task PopulateDropDownsAsync(
            int? currentImporterId = null, 
            int? currentSupplierId = null, 
            int? currentCountryId = null, 
            int? currentCurrencyId = null)
        {
            var importers = await _importerService.GetAllAsync();
            var suppliers = await _supplierService.GetAllAsync();
            var countries = await _countryService.GetAllAsync();
            var currencies = await _currencyService.GetAllAsync();

            var importerItems = importers
                .Where(x => x.IsActive || x.Id == currentImporterId)
                .Select(x => new {
                    x.Id,
                    DisplayName = x.IsActive ? x.LegalName : $"{x.LegalName} (Inactivo)"
                });

            var supplierItems = suppliers
                .Where(x => x.IsActive || x.Id == currentSupplierId)
                .Select(x => new {
                    x.Id,
                    DisplayName = x.IsActive ? x.Name : $"{x.Name} (Inactivo)"
                });

            var countryItems = countries
                .Where(x => x.IsActive || x.Id == currentCountryId)
                .Select(x => new {
                    x.Id,
                    DisplayName = x.IsActive ? x.Name : $"{x.Name} (Inactivo)"
                });

            var currencyItems = currencies
                .Where(x => x.IsActive || x.Id == currentCurrencyId)
                .Select(x => new {
                    x.Id,
                    DisplayName = x.IsActive ? x.IsoCode : $"{x.IsoCode} (Inactivo)"
                });

            ViewBag.Importers = new SelectList(importerItems, "Id", "DisplayName");
            ViewBag.Suppliers = new SelectList(supplierItems, "Id", "DisplayName");
            ViewBag.Countries = new SelectList(countryItems, "Id", "DisplayName");
            ViewBag.Currencies = new SelectList(currencyItems, "Id", "DisplayName");
        }
    }
}
