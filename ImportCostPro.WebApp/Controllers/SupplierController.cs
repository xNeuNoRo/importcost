using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.SupplierViewModels;
using ImportCostPro.Application.DTOs.Supplier.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mapster;

namespace ImportCostPro.WebApp.Controllers
{
    public class SupplierController : Controller
    {
        private readonly SupplierService _supplierService;
        private readonly CountryService _countryService;
        private readonly CurrencyService _currencyService;

        public SupplierController(
            SupplierService supplierService,
            CountryService countryService,
            CurrencyService currencyService)
        {
            _supplierService = supplierService;
            _countryService = countryService;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllAsync();
            var viewModel = suppliers.Adapt<IEnumerable<SupplierListViewModel>>();
            ViewData["Title"] = "Gestión de Proveedores";
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropDownsAsync();
            ViewData["Title"] = "Registrar Nuevo Proveedor";
            return View(new CreateSupplierViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSupplierViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync();
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateSupplierRequest>();
                await _supplierService.CreateAsync(request);
                TempData["SuccessMessage"] = "Proveedor registrado correctamente.";
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
                ModelState.AddModelError(string.Empty, "Error inesperado al registrar el proveedor.");
            }

            await PopulateDropDownsAsync();
            ViewData["Title"] = "Registrar Nuevo Proveedor";
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "El proveedor solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = supplier.Adapt<UpdateSupplierViewModel>();
            await PopulateDropDownsAsync(viewModel.OriginCountryId, viewModel.DefaultCurrencyId);
            ViewData["Title"] = "Editar Proveedor";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateSupplierViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync(viewModel.OriginCountryId, viewModel.DefaultCurrencyId);
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateSupplierRequest>();
                await _supplierService.UpdateAsync(request);
                TempData["SuccessMessage"] = "Proveedor actualizado correctamente.";
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
                ModelState.AddModelError(string.Empty, "Error crítico al actualizar el proveedor.");
            }

            await PopulateDropDownsAsync(viewModel.OriginCountryId, viewModel.DefaultCurrencyId);
            ViewData["Title"] = "Editar Proveedor";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _supplierService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Proveedor eliminado con éxito.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al intentar eliminar el proveedor.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _supplierService.ToggleStatusAsync(id);
                TempData["SuccessMessage"] = "Estado del proveedor actualizado.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropDownsAsync(int? currentCountryId = null, int? currentCurrencyId = null)
        {
            var countries = await _countryService.GetAllAsync();
            var currencies = await _currencyService.GetAllAsync();
            
            ViewBag.Countries = new SelectList(countries.Where(c => c.IsActive || c.Id == currentCountryId), "Id", "Name");
            ViewBag.Currencies = new SelectList(currencies.Where(c => c.IsActive || c.Id == currentCurrencyId), "Id", "IsoCode");
        }
    }
}
