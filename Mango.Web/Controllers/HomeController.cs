using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
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


        // HttpPost for ProductDetails
        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto product)
        {
            CartDto cartDto = new()
            {
                CartHeader = new()
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetailsDto = new()
            {
                Count = product.Count,
                ProductId = product.ProductId
            };

            List<CartDetailsDto> cartDetails = new() { cartDetailsDto };

            cartDto.CartDetails = cartDetails;

            ResponseDto? response = await _cartService.UpsertCartAsync(cartDto);

            if(response != null && response.IsSuccess )
            {
                TempData["success"] = "Item has been added to shopping cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return View(product);
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
