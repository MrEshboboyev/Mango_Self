using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> list = new();
            ResponseDto? response = await _productService.GetAllProductsAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }

            else
            {
                TempData["error"] = response.Message;
            }
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto productDto)
        {
            ResponseDto? response = await _productService.CreateProductAsync(productDto);
            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(productDto);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDelete(int id)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(id);
            if(response != null && response.IsSuccess)
            {
                ProductDto product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result)); 
                return View(product);
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto productDto)
        {
            ResponseDto? response = await _productService.DeleteProductAsync(productDto.ProductId);
            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View();
        }
    }
}
