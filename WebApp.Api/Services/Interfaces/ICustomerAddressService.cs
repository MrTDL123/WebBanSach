using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Address;

namespace WebApp.Api.Services.Interfaces
{
    public interface ICustomerAddressService
    {
        Task<ApiResponse<List<AddressDto>>> GetMyAddressesAsync(string userId);
        Task<ApiResponse<AddressDto>> CreateAddressAsync(string userId, CreateAddressDto dto);
        Task<ApiResponse<AddressDto>> UpdateAddressAsync(string userId, UpdateAddressDto dto);
        Task<ApiResponse<bool>> DeleteAddressAsync(string userId, int addressId);
    }
}
