using AutoMapper;
using Mango.Services.CouponAPI.Data;
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
        private readonly IMapper _mapper;

        public CouponAPIController(AppDbContext db, IMapper mapper)
        {
           _db = db;
            _mapper = mapper;
           _response = new ResponseDto();
        }


        // get all entities
        [HttpGet]
        public ResponseDto? Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
            }
            catch(Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        // get entity by id 
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto? Get(int id)
        {

            try
            {
                Coupon coupon = _db.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon != null)
                {
                    _response.Result = _mapper.Map<CouponDto>(coupon);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Not Found!";
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        // get by code of coupon 
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto? Get(string code)
        {
            try
            {
                Coupon coupon = _db.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
                if (coupon != null) _response.Result = _mapper.Map<CouponDto>(coupon);
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Not found!";
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
