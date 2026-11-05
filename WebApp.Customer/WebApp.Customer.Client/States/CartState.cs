using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Shared.Dtos.CustomerDtos.Cart;

namespace WebApp.Customer.Client.States
{
    /// <summary>
    /// Quản lý trạng thái giỏ hàng thuần C# trong bộ nhớ Client (In-memory State Container).
    /// Thay thế hoàn toàn cơ chế đọc/ghi giỏ hàng bằng jQuery và Session MVC cũ.
    /// Giúp Header, Cart Drawer và nút Thêm vào giỏ đồng bộ tức thì, 0 reload.
    /// </summary>
    public class CartState
    {
        private readonly ICartClientService _cartService;

        public CartState(ICartClientService cartService)
        {
            _cartService = cartService;
        }

        public int TotalItems { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsLoading { get; private set; }
        public bool IsInitialized { get; private set; }

        public event Action? OnChange;

        public async Task InitializeAsync()
        {
            if (IsInitialized) return;
            await RefreshCartAsync();
            IsInitialized = true;
        }

        public async Task RefreshCartAsync()
        {
            IsLoading = true;
            NotifyStateChanged();

            try
            {
                var response = await _cartService.GetCartAsync();
                if (response.Success && response.Data != null)
                {
                    TotalItems = response.Data.TotalItems;
                    TotalAmount = response.Data.TotalAmount;
                }
                else
                {
                    TotalItems = 0;
                    TotalAmount = 0;
                }
            }
            catch
            {
                TotalItems = 0;
                TotalAmount = 0;
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }

        public async Task<bool> AddToCartAsync(int productId, int quantity = 1)
        {
            IsLoading = true;
            NotifyStateChanged();

            try
            {
                var response = await _cartService.AddToCartAsync(new AddToCartDto(productId, quantity));
                if (response.Success && response.Data != null)
                {
                    TotalItems = response.Data.TotalItems;
                    TotalAmount = response.Data.TotalAmount;
                    return true;
                }
                return false;
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }

        public void UpdateSummary(int totalItems, decimal totalAmount)
        {
            TotalItems = totalItems;
            TotalAmount = totalAmount;
            NotifyStateChanged();
        }

        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
