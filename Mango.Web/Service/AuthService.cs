using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public Task<ResponseDto> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            throw new NotImplementedException();
        }
    }
}
