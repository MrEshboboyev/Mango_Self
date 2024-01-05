using Mango.Web.Models;
using Mango.Web.Service.IService;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        public Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            throw new NotImplementedException();
        }
    }
}
