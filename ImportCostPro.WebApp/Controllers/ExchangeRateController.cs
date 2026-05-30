using ImportCostPro.Application.DTOs.ExchangeRate.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.ExchageRateViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.WebApp.Controllers
{
    public class ExchangeRateController : Controller
    {
        private readonly ExchangeRateService _exchangeRateService;
        private readonly CurrencyService _currencyService;

        public ExchangeRateController(
            ExchangeRateService exchangeRateService, 
            CurrencyService currencyService)
        {
            _exchangeRateService = exchangeRateService;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var rates = await _exchangeRateService.GetAllAsync();
            var viewModel = rates.Adapt<IEnumerable<ExchangeRateViewModel>>();
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCurrenciesAsync();
            return View(new CreateExchangeRateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExchangeRateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCurrenciesAsync();
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateExchangeRateRequest>();
                await _exchangeRateService.CreateAsync(request);
                TempData["SuccessMessage"] = "Tasa de cambio registrada correctamente.";
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
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al registrar la tasa.";
            }

            await PopulateCurrenciesAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var rate = await _exchangeRateService.GetByIdAsync(id);
            if (rate == null)
            {
                TempData["ErrorMessage"] = "La tasa solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = rate.Adapt<UpdateExchangeRateViewModel>();
            await PopulateCurrenciesAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateExchangeRateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCurrenciesAsync();
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateExchangeRateRequest>();
                await _exchangeRateService.UpdateAsync(request);
                TempData["SuccessMessage"] = "Tasa de cambio actualizada correctamente.";
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
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al actualizar la tasa.";
            }

            await PopulateCurrenciesAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _exchangeRateService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Tasa de cambio eliminada con éxito.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar la tasa.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _exchangeRateService.ToggleStatusAsync(id);
                TempData["SuccessMessage"] = "Estado de la tasa de cambio actualizado correctamente.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCurrenciesAsync()
        {
            var currencies = await _currencyService.GetAllAsync();
            var activeCurrencies = currencies.Where(c => c.IsActive);
            
            ViewBag.Currencies = new SelectList(activeCurrencies, "Id", "Name");
        }
    }
}
