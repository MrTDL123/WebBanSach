using Microsoft.AspNetCore.Identity;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;

namespace WebApp.Api.Endpoints
{
    /// <summary>
    /// Các Endpoints phục vụ kiểm tra trạng thái xác thực
    /// </summary>
    public static class AuthEndpoints
    {
        public static void MapRevalidateStateEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/auth/validate-stamps", async (
                ICurrentUserService currentUser,
                UserManager<User> userManager) =>
            {
                if (!currentUser.IsAuthenticated || string.IsNullOrEmpty(currentUser.UserId))
                {
                    return Results.Unauthorized();
                }

                var dbUser = await userManager.FindByIdAsync(currentUser.UserId);
                if (dbUser == null || !dbUser.IsActive)
                {
                    return Results.Unauthorized();
                }

                string currentStamp = await userManager.GetSecurityStampAsync(dbUser);
                return Results.Ok(new { SecurityStamp = currentStamp });
            })
            .RequireAuthorization(); // Việc kiểm tra theo định kỳ chỉ diễn ra nếu người dùng đã được xác thực, phân quyền hợp lệ
        }
    }
}
