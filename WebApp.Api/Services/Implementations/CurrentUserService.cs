using System.Security.Claims;
using WebApp.Api.Services.Interfaces;

namespace WebApp.Api.Services.Implementations
{
    /// <summary>
    /// Service được sử dụng để lấy thông tin user trong session hiện tại
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    } 
}
