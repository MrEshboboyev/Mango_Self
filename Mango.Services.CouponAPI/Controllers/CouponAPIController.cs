﻿using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;

        public CouponAPIController(AppDbContext db)
        {
           _db = db;
           _response = new ResponseDto();
        }


        // get all entities
        [HttpGet]
        public IEnumerable<Coupon> Get()
        {
            IEnumerable<Coupon> objList = _db.Coupons.ToList();
            return objList;
        }

        // get entity by id 
        [HttpGet]
        [Route("{id:int}")]
        public Coupon Get(int id)
        {
            Coupon obj = _db.Coupons.FirstOrDefault(c => c.CouponId == id);
            if(obj == null)
            {
                return new Coupon();
            }
            return obj;
        }
    }
}