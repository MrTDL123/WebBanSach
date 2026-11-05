using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Order;

namespace WebApp.Api.Services.Interfaces
{
    public interface ICustomerOrderService
    {
        Task<ApiResponse<VoucherApplyResultDto>> ApplyVoucherAsync(ApplyVoucherDto dto);
        Task<ApiResponse<OrderDto>> CheckoutAsync(string userId, CreateOrderDto dto);
        Task<ApiResponse<List<OrderListItemDto>>> GetMyOrdersAsync(string userId, string? status = null);
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(string userId, int orderId);
        Task<ApiResponse<bool>> CancelOrderAsync(string userId, int orderId, string? cancelReason = null);
    }
}
