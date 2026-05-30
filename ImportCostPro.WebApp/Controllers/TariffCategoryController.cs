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
            var viewModel = categories.Adapt<IEnumerable<TariffCategoryViewModel>>();
            ViewData["Title"] = "Categorías Arancelarias";
            return View(viewModel);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Nueva Categoría Arancelaria";
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
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al registrar la categoría arancelaria.";
            }

            ViewData["Title"] = "Nueva Categoría Arancelaria";
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

            var viewModel = category.Adapt<UpdateTariffCategoryViewModel>();
            ViewData["Title"] = "Editar Categoría Arancelaria";
            return View(viewModel);
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
                TempData["SuccessMessage"] = "Categoría arancelaria actualizada correctamente.";
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
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al actualizar la categoría arancelaria.";
            }

            ViewData["Title"] = "Editar Categoría Arancelaria";
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

            return RedirectToAction(nameof(Index));
        }
    }
}
