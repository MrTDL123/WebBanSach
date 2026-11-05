using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Api.Data;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Address;

namespace WebApp.Api.Services.Implementations
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly AppDbContext _context;

        public CustomerAddressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<AddressDto>>> GetMyAddressesAsync(string userId)
        {
            var addresses = await _context.CustomerAddresses
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.AddressId)
                .Select(a => new AddressDto(
                    a.AddressId,
                    a.ReceiverName,
                    a.ReceiverPhone,
                    a.Province,
                    a.District,
                    a.Ward,
                    a.DetailAddress,
                    a.IsDefault
                ))
                .ToListAsync();

            return ApiResponse<List<AddressDto>>.SuccessResult(addresses);
        }

        public async Task<ApiResponse<AddressDto>> CreateAddressAsync(string userId, CreateAddressDto dto)
        {
            var userAddresses = await _context.CustomerAddresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            // Nếu đây là địa chỉ đầu tiên hoặc khách hàng tick chọn SetAsDefault
            bool shouldSetDefault = dto.SetAsDefault || !userAddresses.Any();

            if (shouldSetDefault && userAddresses.Any())
            {
                foreach (var addr in userAddresses.Where(a => a.IsDefault))
                {
                    addr.IsDefault = false;
                }
            }

            var newAddress = new CustomerAddress
            {
                UserId = userId,
                ReceiverName = dto.ReceiverName.Trim(),
                ReceiverPhone = dto.ReceiverPhone.Trim(),
                Province = dto.Province?.Trim() ?? string.Empty,
                District = dto.District?.Trim() ?? string.Empty,
                Ward = dto.Ward?.Trim() ?? string.Empty,
                DetailAddress = dto.DetailAddress.Trim(),
                IsDefault = shouldSetDefault
            };

            _context.CustomerAddresses.Add(newAddress);
            await _context.SaveChangesAsync();

            var result = new AddressDto(
                newAddress.AddressId,
                newAddress.ReceiverName,
                newAddress.ReceiverPhone,
                newAddress.Province,
                newAddress.District,
                newAddress.Ward,
                newAddress.DetailAddress,
                newAddress.IsDefault
            );

            return ApiResponse<AddressDto>.SuccessResult(result, "Thêm địa chỉ nhận hàng thành công.");
        }

        public async Task<ApiResponse<AddressDto>> UpdateAddressAsync(string userId, UpdateAddressDto dto)
        {
            var address = await _context.CustomerAddresses
                .FirstOrDefaultAsync(a => a.AddressId == dto.AddressId && a.UserId == userId);

            if (address == null)
            {
                return ApiResponse<AddressDto>.FailureResult("Không tìm thấy địa chỉ hoặc địa chỉ không thuộc quyền sở hữu của bạn.");
            }

            if (dto.SetAsDefault && !address.IsDefault)
            {
                var otherAddresses = await _context.CustomerAddresses
                    .Where(a => a.UserId == userId && a.AddressId != dto.AddressId && a.IsDefault)
                    .ToListAsync();

                foreach (var other in otherAddresses)
                {
                    other.IsDefault = false;
                }

                address.IsDefault = true;
            }

            address.ReceiverName = dto.ReceiverName.Trim();
            address.ReceiverPhone = dto.ReceiverPhone.Trim();
            address.Province = dto.Province?.Trim() ?? string.Empty;
            address.District = dto.District?.Trim() ?? string.Empty;
            address.Ward = dto.Ward?.Trim() ?? string.Empty;
            address.DetailAddress = dto.DetailAddress.Trim();

            await _context.SaveChangesAsync();

            var result = new AddressDto(
                address.AddressId,
                address.ReceiverName,
                address.ReceiverPhone,
                address.Province,
                address.District,
                address.Ward,
                address.DetailAddress,
                address.IsDefault
            );

            return ApiResponse<AddressDto>.SuccessResult(result, "Cập nhật địa chỉ nhận hàng thành công.");
        }

        public async Task<ApiResponse<bool>> DeleteAddressAsync(string userId, int addressId)
        {
            var address = await _context.CustomerAddresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

            if (address == null)
            {
                return ApiResponse<bool>.FailureResult("Không tìm thấy địa chỉ để xóa.");
            }

            bool wasDefault = address.IsDefault;
            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();

            // Nếu vừa xóa địa chỉ mặc định, tự động chuyển mặc định sang địa chỉ đầu tiên còn lại (nếu có)
            if (wasDefault)
            {
                var remaining = await _context.CustomerAddresses
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.AddressId)
                    .FirstOrDefaultAsync();

                if (remaining != null)
                {
                    remaining.IsDefault = true;
                    await _context.SaveChangesAsync();
                }
            }

            return ApiResponse<bool>.SuccessResult(true, "Xóa địa chỉ thành công.");
        }
    }
}
