using ImportCostPro.Application.DTOs.Country.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.CountryViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.WebApp.Controllers
{
    public class CountryController : Controller
    {
        private readonly CountryService _countryService;

        public CountryController(CountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<IActionResult> Index()
        {
            var countries = await _countryService.GetAllAsync();
            var viewModel = countries.Adapt<IEnumerable<CountryViewModel>>();
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new CountryCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateCountryRequest>();
                await _countryService.CreateAsync(request);
                TempData["SuccessMessage"] = "País registrado correctamente.";
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
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al registrar el país.";
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var country = await _countryService.GetByIdAsync(id);
            if (country == null)
            {
                TempData["ErrorMessage"] = "El país solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = country.Adapt<CountryUpdateViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CountryUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateCountryRequest>();
                await _countryService.UpdateAsync(request);
                TempData["SuccessMessage"] = "País actualizado correctamente.";
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
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al actualizar el país.";
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _countryService.DeleteAsync(id);
                TempData["SuccessMessage"] = "País eliminado con éxito.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar el país.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _countryService.ToggleStatusAsync(id);
                TempData["SuccessMessage"] = "Estado del país actualizado correctamente.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al cambiar el estado del país.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
