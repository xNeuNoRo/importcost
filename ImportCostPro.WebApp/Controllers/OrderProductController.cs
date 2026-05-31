using ImportCostPro.Application.DTOs.OrderProduct.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Services;
using ImportCostPro.Application.ViewModels.ImportOrderViewModels;
using ImportCostPro.Application.ViewModels.OrderProductViewModels;
using ImportCostPro.Persistence.Enums;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.WebApp.Controllers
{
    public class OrderProductController : Controller
    {
        private readonly OrderProductService _orderProductService;
        private readonly ImportOrderService _importOrderService;
        private readonly ProductService _productService;

        public OrderProductController(
            OrderProductService orderProductService,
            ImportOrderService importOrderService,
            ProductService productService)
        {
            _orderProductService = orderProductService;
            _importOrderService = importOrderService;
            _productService = productService;
        }

        public async Task<IActionResult> Manage(int orderId, string? source = null)
        {
            var order = await _importOrderService.GetByIdAsync(orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "La orden solicitada no existe.";
                return RedirectToAction("Index", "ImportOrder");
            }

            var items = await _orderProductService.GetProductsByOrderIdAsync(orderId);
            ViewBag.Order = order.Adapt<ImportOrderViewModel>();
            ViewBag.Source = source;
            ViewData["Title"] = $"Productos - Orden {order.OrderNumber}";
            
            return View(items.Adapt<IEnumerable<OrderProductViewModel>>());
        }

        public async Task<IActionResult> Add(int orderId, string? source = null)
        {
            var order = await _importOrderService.GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Open)
            {
                TempData["ErrorMessage"] = "La orden no permite agregar productos.";
                return RedirectToAction(nameof(Manage), new { orderId, source });
            }

            await PopulateProductsAsync();
            ViewBag.OrderId = orderId;
            ViewBag.OrderNumber = order.OrderNumber;
            ViewBag.Source = source;
            return View(new CreateOrderProductViewModel { ImportOrderId = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateOrderProductViewModel viewModel, string? source = null)
        {
            if (!ModelState.IsValid)
            {
                await PopulateProductsAsync();
                ViewBag.OrderId = viewModel.ImportOrderId;
                ViewBag.Source = source;
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<CreateOrderProductRequest>();
                await _orderProductService.CreateAsync(request);
                TempData["SuccessMessage"] = "Producto añadido a la orden.";
                return RedirectToAction(nameof(Manage), new { orderId = viewModel.ImportOrderId, source });
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
                ModelState.AddModelError(string.Empty, "Error inesperado al añadir el producto.");
            }

            await PopulateProductsAsync();
            ViewBag.OrderId = viewModel.ImportOrderId;
            ViewBag.Source = source;
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id, string? source = null)
        {
            var item = await _orderProductService.GetByIdAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "El registro solicitado no existe.";
                return RedirectToAction("Index", "ImportOrder");
            }

            var order = await _importOrderService.GetByIdAsync(item.ImportOrderId);
            if (order == null || order.Status != OrderStatus.Open)
            {
                TempData["ErrorMessage"] = "Esta orden está bloqueada para modificaciones.";
                return RedirectToAction(nameof(Manage), new { orderId = item.ImportOrderId, source });
            }

            ViewBag.ProductName = item.ProductName;
            ViewBag.OrderNumber = order.OrderNumber;
            ViewBag.OrderId = item.ImportOrderId;
            ViewBag.Source = source;

            return View(item.Adapt<UpdateOrderProductViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateOrderProductViewModel viewModel, string? source = null)
        {
            var item = await _orderProductService.GetByIdAsync(viewModel.Id);
            int orderId = item?.ImportOrderId ?? 0;

            if (!ModelState.IsValid)
            {
                ViewBag.ProductName = item?.ProductName;
                ViewBag.OrderId = orderId;
                ViewBag.Source = source;
                return View(viewModel);
            }

            try
            {
                var request = viewModel.Adapt<UpdateOrderProductRequest>();
                await _orderProductService.UpdateAsync(request);
                TempData["SuccessMessage"] = "Línea de producto actualizada.";
                return RedirectToAction(nameof(Manage), new { orderId, source });
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
                ModelState.AddModelError(string.Empty, "Error al actualizar la línea de producto.");
            }

            ViewBag.ProductName = item?.ProductName;
            ViewBag.OrderId = orderId;
            ViewBag.Source = source;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? source = null)
        {
            var item = await _orderProductService.GetByIdAsync(id);
            if (item == null) return RedirectToAction("Index", "ImportOrder");

            int orderId = item.ImportOrderId;
            try
            {
                await _orderProductService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Producto removido de la orden.";
            }
            catch (BusinessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al intentar remover el producto.";
            }

            return RedirectToAction(nameof(Manage), new { orderId, source });
        }

        private async Task PopulateProductsAsync()
        {
            var products = await _productService.GetAllAsync();
            ViewBag.Products = new SelectList(products.Where(p => p.IsActive), "Id", "Name");
        }
    }
}
