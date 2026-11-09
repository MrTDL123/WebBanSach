using Microsoft.AspNetCore.Components.Forms;
using WebApp.Customer.Client.Services.Base;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Cart;

namespace WebApp.Customer.Client.Services.Implementations
{
    public class CartClientService : BaseApiClient, ICartClientService
    {
        public CartClientService(
            HttpClient httpClient,
            ICookieProvider? cookieProvider = null,
            AntiforgeryStateProvider? antiforgeryStateProvider = null)
            : base(httpClient, cookieProvider, antiforgeryStateProvider)
        {
        }

        public async Task<ApiResponse<CartDto>> GetCartAsync()
        {
            return await GetAsync<CartDto>("api/cart");
        }

        public async Task<ApiResponse<CartDto>> AddToCartAsync(AddToCartDto dto)
        {
            return await PostAsJsonAsync<AddToCartDto, CartDto>("api/cart/items", dto);
        }

        public async Task<ApiResponse<CartDto>> UpdateCartItemAsync(UpdateCartItemDto dto)
        {
            return await PutAsJsonAsync<UpdateCartItemDto, CartDto>("api/cart/items", dto);
        }

        public async Task<ApiResponse<CartDto>> RemoveCartItemAsync(int productId)
        {
            return await DeleteAsync<CartDto>($"api/cart/items/{productId}");
        }

        public async Task<ApiResponse<bool>> ClearCartAsync()
        {
            return await DeleteAsync<bool>("api/cart");
        }
    }
}
