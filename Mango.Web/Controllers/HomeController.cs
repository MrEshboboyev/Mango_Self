using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            ResponseDto response = await _productService.GetAllProductsAsync();
            if(response != null && response.IsSuccess)
            {
                IEnumerable<ProductDto> objList = JsonConvert.DeserializeObject<IEnumerable<ProductDto>>
                    (Convert.ToString(response.Result));

                return View(objList);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);
            if(response != null && response.IsSuccess)
            {
                ProductDto obj = JsonConvert.DeserializeObject<ProductDto>
                    (Convert.ToString(response.Result));

                return View(obj);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
