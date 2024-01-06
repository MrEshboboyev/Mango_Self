﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> list = new();
            ResponseDto? response = await _couponService.GetAllCouponsAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto couponDto)
        {
            ResponseDto? response = await _couponService.CreateCouponAsync(couponDto);
            if(response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CouponIndex));
            }
            return View(couponDto);
        }

        [HttpGet]
        public async Task<IActionResult> CouponDelete(int id)
        {
            ResponseDto? response = await _couponService.GetCouponByIdAsync(id);
            if(response != null && response.IsSuccess)
            {
                CouponDto coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result)); 
                return View(coupon);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto couponDto)
        {
            ResponseDto? response = await _couponService.DeleteCouponAsync(couponDto.CouponId);
            if(response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CouponIndex));   
            }
            return View();
        }
    }
}
