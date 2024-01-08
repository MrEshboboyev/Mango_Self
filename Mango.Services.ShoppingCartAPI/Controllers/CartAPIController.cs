using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private ResponseDto _response;

        public CartAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            this._response = new ResponseDto();
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto?> CartUpsert([FromBody] CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = _db.CartHeaders.FirstOrDefault(u => u.UserId == 
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
                    var cartDetailsFromDb = _db.CartDetails.FirstOrDefault(u => u.ProductId ==
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
                    }
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
