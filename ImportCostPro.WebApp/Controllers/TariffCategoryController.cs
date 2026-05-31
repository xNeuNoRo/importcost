using ImportCostPro.Application.DTOs.TariffCategory.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.TariffCategoryViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.WebApp.Controllers
{
    public class TariffCategoryController : Controller
    {
        private readonly TariffCategoryService _tariffCategoryService;

        public TariffCategoryController(TariffCategoryService tariffCategoryService)
        {
            _tariffCategoryService = tariffCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _tariffCategoryService.GetAllAsync();
            return View(categories.Adapt<IEnumerable<TariffCategoryViewModel>>());
        }

        public IActionResult Create()
        {
            return View(new CreateTariffCategoryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTariffCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateTariffCategoryRequest>();
                await _tariffCategoryService.CreateAsync(request);
                TempData["SuccessMessage"] = "Categoría arancelaria registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationBusinessException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }
            catch (BusinessException ex)
            {
                // No usamos TempData aquí porque regresamos la misma vista
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al registrar la categoría arancelaria.");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _tariffCategoryService.GetByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "La categoría arancelaria solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(category.Adapt<UpdateTariffCategoryViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTariffCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateTariffCategoryRequest>();
                await _tariffCategoryService.UpdateAsync(request);
                TempData["SuccessMessage"] = "Categoría arancelaria actualizada.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationBusinessException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }
            catch (BusinessException ex)
            {
                // No usamos TempData aquí porque regresamos la misma vista
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar la categoría arancelaria.");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _tariffCategoryService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Categoría arancelaria eliminada con éxito.";
            }
            catch (BusinessException ex)
            {
                // Aquí SÍ usamos TempData porque redirigimos
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error inesperado al intentar eliminar la categoría arancelaria.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _tariffCategoryService.ToggleStatusAsync(id);
                TempData["SuccessMessage"] = "Estado de la categoría arancelaria actualizado.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al cambiar el estado.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
