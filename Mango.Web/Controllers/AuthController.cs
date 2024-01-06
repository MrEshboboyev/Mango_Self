using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            return View();
        }
    }
}
