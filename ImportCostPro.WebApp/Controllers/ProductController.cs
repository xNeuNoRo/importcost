using ImportCostPro.Application.DTOs.Product.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.ProductViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CountryService _countryService;
        private readonly TariffCategoryService _tariffCategoryService;

        public ProductController(
            ProductService productService,
            CountryService countryService,
            TariffCategoryService tariffCategoryService)
        {
            _productService = productService;
            _countryService = countryService;
            _tariffCategoryService = tariffCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            var viewModel = products.Adapt<IEnumerable<ProductViewModel>>();
            ViewData["Title"] = "Catálogo de Productos";
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropDownsAsync();
            ViewData["Title"] = "Registrar Nuevo Producto";
            return View(new ProductCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync();
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateProductRequest>();
                await _productService.CreateAsync(request);
                TempData["SuccessMessage"] = "Producto registrado exitosamente en el catálogo.";
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
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al registrar el producto.");
            }

            await PopulateDropDownsAsync();
            ViewData["Title"] = "Registrar Nuevo Producto";
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "El producto solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = product.Adapt<ProductUpdateViewModel>();
            await PopulateDropDownsAsync();
            ViewData["Title"] = "Editar Producto";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync();
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateProductRequest>();
                await _productService.UpdateAsync(request);
                TempData["SuccessMessage"] = "Producto actualizado correctamente.";
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
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar el producto.");
            }

            await PopulateDropDownsAsync();
            ViewData["Title"] = "Editar Producto";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Producto eliminado del catálogo.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error inesperado al intentar eliminar el producto.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _productService.ToggleStatusAsync(id);
                TempData["SuccessMessage"] = "Estado del producto actualizado.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropDownsAsync()
        {
            var countries = await _countryService.GetAllAsync();
            var categories = await _tariffCategoryService.GetAllAsync();
            
            ViewBag.Countries = new SelectList(countries.Where(c => c.IsActive), "Id", "Name");
            ViewBag.Categories = new SelectList(categories.Where(c => c.IsActive), "Id", "Code");
        }
    }
}
