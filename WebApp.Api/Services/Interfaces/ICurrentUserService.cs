using System.Security.Claims;

namespace WebApp.Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        ClaimsPrincipal? User {  get; }
        bool IsAuthenticated { get; }
    }
}
