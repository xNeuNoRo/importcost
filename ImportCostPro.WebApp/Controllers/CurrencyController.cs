using ImportCostPro.Application.DTOs.Currency.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.CurrencyViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.WebApp.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly CurrencyService _currencyService;

        public CurrencyController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var currencies = await _currencyService.GetAllAsync();
            var viewModel = currencies.Adapt<IEnumerable<CurrencyViewModel>>();
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new CreateCurrencyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCurrencyViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateCurrencyRequest>();
                await _currencyService.CreateAsync(request);
                TempData["SuccessMessage"] = "Moneda creada exitosamente.";
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
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear la moneda.");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currency = await _currencyService.GetByIdAsync(id);
            if (currency == null)
            {
                TempData["ErrorMessage"] = "La moneda solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = currency.Adapt<UpdateCurrencyViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCurrencyViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateCurrencyRequest>();
                await _currencyService.UpdateAsync(request);
                TempData["SuccessMessage"] = "Moneda actualizada exitosamente.";
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
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar la moneda.");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var currency = await _currencyService.GetByIdAsync(id);
            if (currency == null)
            {
                TempData["ErrorMessage"] = "La moneda solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = currency.Adapt<CurrencyViewModel>();
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _currencyService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Moneda eliminada exitosamente.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al eliminar la moneda.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _currencyService.ToggleStatusAsync(id);
                TempData["SuccessMessage"] = "Estado de la moneda actualizado exitosamente.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al cambiar el estado de la moneda.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
