using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Cart;

namespace WebApp.Customer.Client.Services.Interfaces
{
    public interface ICartClientService
    {
        Task<ApiResponse<CartDto>> GetCartAsync();
        Task<ApiResponse<CartDto>> AddToCartAsync(AddToCartDto dto);
        Task<ApiResponse<CartDto>> UpdateCartItemAsync(UpdateCartItemDto dto);
        Task<ApiResponse<CartDto>> RemoveCartItemAsync(int productId);
        Task<ApiResponse<bool>> ClearCartAsync();
    }
}
