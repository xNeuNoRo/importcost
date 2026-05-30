using ImportCostPro.Application.DTOs.TaxConfiguration.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.TaxConfigurationViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.WebApp.Controllers
{
    public class TaxConfigurationController : Controller
    {
        private readonly TaxConfigurationService _taxConfigurationService;

        public TaxConfigurationController(TaxConfigurationService taxConfigurationService)
        {
            _taxConfigurationService = taxConfigurationService;
        }

        public async Task<IActionResult> Index()
        {
            var config = await _taxConfigurationService.GetConfigurationAsync();
            var viewModel = config.Adapt<SaveTaxConfigurationViewModel>() ?? new SaveTaxConfigurationViewModel();
            
            ViewData["Title"] = "Configuración de Impuestos";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SaveTaxConfigurationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", viewModel);
            }

            try
            {
                var request = viewModel.Adapt<SaveTaxConfigurationRequest>();
                await _taxConfigurationService.SaveConfigurationAsync(request);
                TempData["SuccessMessage"] = "Configuración de impuestos actualizada correctamente.";
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
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al guardar la configuración.";
            }

            return View("Index", viewModel);
        }
    }
}
