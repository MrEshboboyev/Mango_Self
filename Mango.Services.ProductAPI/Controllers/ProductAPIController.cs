using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    //[Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private readonly IMapper _mapper;

        public ProductAPIController(AppDbContext db, IMapper mapper)
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
                IEnumerable<Product> objList = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);
            }
            catch (Exception ex)
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
                Product product = _db.Products.FirstOrDefault(c => c.ProductId == id);
                if (product != null)
                {
                    _response.Result = _mapper.Map<ProductDto>(product);
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


        // creating entity
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto? Post([FromBody] ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                _db.Products.Add(product);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // updating entity
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto? Put([FromBody] ProductDto productDto)
        {
            try
            {
                Product productFromDb = _db.Products.AsNoTracking().FirstOrDefault(c => c.ProductId == productDto.ProductId);
                if (productFromDb != null)
                {
                    productFromDb = _mapper.Map<Product>(productDto);
                    _db.Products.Update(productFromDb);
                    _db.SaveChanges();

                    _response.Result = _mapper.Map<ProductDto>(productFromDb);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Not found";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // deleting entity
        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto? Delete([FromBody] int id)
        {
            try
            {
                Product productFromDb = _db.Products.FirstOrDefault(c => c.ProductId == id);
                if (productFromDb != null)
                {
                    _db.Products.Remove(productFromDb);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
