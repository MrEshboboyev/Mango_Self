﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
             _cartService = cartService;
        }


        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if(response != null && response.IsSuccess) 
            {
                TempData["success"] = "Product removed successfully";
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon Applied successfully";
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return RedirectToAction(nameof(CartIndex));
        }

        // sending email cart request
        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == 
                JwtRegisteredClaimNames.Email).First().Value;
            ResponseDto? response = await _cartService.EmailCart(cartDto);
            if(response != null && response.IsSuccess )
            {
                TempData["success"] = "Email will be proceed and sent shortly!";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon removed successfully";
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return RedirectToAction(nameof(CartIndex));
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).First().Value;

            ResponseDto? response = await _cartService.GetCardByUserIdAsync(userId);
            if(response != null && response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(
                    Convert.ToString(response.Result));

                return cartDto;
            }

            return new CartDto();
        }
    }
}
