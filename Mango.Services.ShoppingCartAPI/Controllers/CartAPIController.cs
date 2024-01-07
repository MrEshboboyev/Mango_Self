using AutoMapper;
using Mango.Services.ProductAPI.Data;
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



    }
}
