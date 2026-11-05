using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Order;

namespace WebApp.Api.Services.Interfaces
{
    public interface ICustomerReturnService
    {
        Task<ApiResponse<ReturnRequestSummaryDto>> CreateReturnRequestAsync(string userId, CreateReturnRequestDto dto);
        Task<ApiResponse<List<ReturnRequestSummaryDto>>> GetMyReturnRequestsAsync(string userId);
    }
}
