using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.Importer;
using ImportCostPro.Application.DTOs.Importer.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mapster;

namespace ImportCostPro.WebApp.Controllers
{
    public class ImporterController : Controller
    {
        private readonly ImporterService _importerService;
        private readonly CountryService _countryService;

        public ImporterController(
            ImporterService importerService,
            CountryService countryService)
        {
            _importerService = importerService;
            _countryService = countryService;
        }

        public async Task<IActionResult> Index()
        {
            var importers = await _importerService.GetAllAsync();
            var viewModel = importers.Adapt<IEnumerable<ImporterViewModel>>();
            ViewData["Title"] = "Importadores";
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCountriesAsync();
            ViewData["Title"] = "Registrar Nuevo Importador";
            return View(new ImporterCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImporterCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCountriesAsync();
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateImporterRequest>();
                await _importerService.CreateAsync(request);
                TempData["SuccessMessage"] = "Importador registrado correctamente.";
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
                ModelState.AddModelError(string.Empty, "Error inesperado al registrar el importador.");
            }

            await PopulateCountriesAsync();
            ViewData["Title"] = "Registrar Nuevo Importador";
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var importer = await _importerService.GetByIdAsync(id);
            if (importer == null)
            {
                TempData["ErrorMessage"] = "El importador solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = importer.Adapt<ImporterUpdateViewModel>();
            await PopulateCountriesAsync(viewModel.CountryId);
            ViewData["Title"] = "Editar Importador";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ImporterUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCountriesAsync(viewModel.CountryId);
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateImporterRequest>();
                await _importerService.UpdateAsync(request);
                TempData["SuccessMessage"] = "Importador actualizado correctamente.";
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
                ModelState.AddModelError(string.Empty, "Error crítico al actualizar el importador.");
            }

            await PopulateCountriesAsync(viewModel.CountryId);
            ViewData["Title"] = "Editar Importador";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _importerService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Importador eliminado con éxito.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error inesperado al intentar eliminar el importador.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _importerService.ToggleStatusAsync(id);
                TempData["SuccessMessage"] = "Estado del importador actualizado.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCountriesAsync(int? currentCountryId = null)
        {
            var countries = await _countryService.GetAllAsync();
            ViewBag.Countries = new SelectList(countries.Where(c => c.IsActive || c.Id == currentCountryId), "Id", "Name");
        }
    }
}
