using Microsoft.AspNetCore.Identity;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Auth;

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

                // Trả về cho Blazor để đối chiếu Security Stamp
                string currentStamp = await userManager.GetSecurityStampAsync(dbUser);
                var currentRoles = (await userManager.GetRolesAsync(dbUser)).ToList();
                
                var responseData = new ValidateStampDto(currentStamp, currentRoles);
                return Results.Ok(ApiResponse<ValidateStampDto>.SuccessResult(responseData));
            })
            .RequireAuthorization(); // Việc kiểm tra theo định kỳ chỉ diễn ra nếu người dùng đã được xác thực, phân quyền hợp lệ
        }
    }
}
