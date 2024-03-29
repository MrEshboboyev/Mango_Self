﻿using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private ResponseDto _response;

        public CartAPIController(AppDbContext db, IMapper mapper, 
            IProductService productService, 
            ICouponService couponService,
            IMessageBus messageBus,
            IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _couponService = couponService;
            _productService = productService;
            _messageBus = messageBus;
            _configuration = configuration;
            this._response = new ResponseDto();
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto?> CartUpsert([FromBody] CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = _db.CartHeaders.AsNoTracking().FirstOrDefault(u => u.UserId == 
                    cartDto.CartHeader.UserId);

                // user cart header was not found 
                if (cartHeaderFromDb == null)
                {
                    // create cart header and cart details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);   
                    await _db.SaveChangesAsync();

                    // create cart details same as for cart header
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // if cart header is not null
                    // check if details has same product
                    var cartDetailsFromDb = _db.CartDetails.AsNoTracking().FirstOrDefault(u => u.ProductId ==
                        cartDto.CartDetails.First().ProductId 
                        && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    // same product is not found
                    if (cartDetailsFromDb == null)
                    {
                        // create cart details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        // updating cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>( cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto?> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.FirstOrDefault(c => c.CartDetailsId == cartDetailsId);

                int totalCountOfItem = _db.CartDetails.Where(c =>
                    c.CartHeaderId == cartDetails.CartHeaderId).Count();

                // removing this cart detail
                _db.CartDetails.Remove(cartDetails);

                // if total items equal 1, removing this cart header
                if (totalCountOfItem == 1)
                {
                    CartHeader cartHeader = _db.CartHeaders.FirstOrDefault(c =>
                        c.CartHeaderId == cartDetails.CartHeaderId);

                    if (cartHeader != null)
                    {
                        _db.CartHeaders.Remove(cartHeader);
                    }
                }

                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto?> GetCart(string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.FirstOrDefault(c => c.UserId == userId)),
                };

                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(
                    c => c.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> products = await _productService.GetProducts();

                foreach(var item in cart.CartDetails)
                {
                    item.Product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Product.Price * item.Count);
                }

                // applied coupon for cartDto
                if(!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupons(cart.CartHeader.CouponCode);
                    if(coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto?> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = _db.CartHeaders.FirstOrDefault
                    (c => c.UserId == cartDto.CartHeader.UserId);

                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }


        // email cart request
        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDto> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto,
                    _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto?> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = _db.CartHeaders.FirstOrDefault
                    (c => c.UserId == cartDto.CartHeader.UserId);

                cartHeaderFromDb.CouponCode = "";
                _db.CartHeaders.Update(cartHeaderFromDb);
                await _db.SaveChangesAsync();

                _response.Result = true;
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
