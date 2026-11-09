using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Data;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Api.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AppDbContext _context;

        public AdminService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<ApiResponse<PagedResultDto<UserListItemDto>>> GetCustomersPagedAsync(UserFilterDto filter)
        {
            try
            {
                /*
                    Cách viết Method Syntax Linq ngắn gọn nhưng khó hiểu, 
                        join 2 bảng UserRoles và Roles để lấy danh sách UserId 
                        của các user có role là "Customer"
                 */
                //var customerUserIds = _context.UserRoles
                //    .Where(ur => _roleManager.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Customer"))
                //    .Select(ur => ur.UserId);

                /* 
                    Cách viết QuerySyntax Linq tường minh dễ hiểu join 2 bảng 
                        UserRoles (có sẵn trong AppDbContext) và bảng Roles để lấy danh sách UserId của các user có role là "Customer"
                */
                var customerUserIds = from ur in _context.UserRoles
                                      join r in _roleManager.Roles on ur.RoleId equals r.Id
                                      where r.Name == "Customer"
                                      select ur.UserId;

                var query = _userManager.Users
                    .AsNoTracking()
                    .Where(u => customerUserIds.Contains(u.Id));

                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var term = filter.SearchTerm.Trim().ToLower();
                    query = query.Where(u => (u.UserName != null && u.UserName.ToLower().Contains(term)) ||
                                             (u.Email != null && u.Email.ToLower().Contains(term)) ||
                                             (u.FullName != null && u.FullName.ToLower().Contains(term)) ||
                                             (u.PhoneNumber != null && u.PhoneNumber.Contains(term)));
                }

                if (filter.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == filter.IsActive.Value);
                }

                var totalCount = await query.CountAsync();
                var customerOrders = await _context.Orders
                    .Where(c => customerUserIds.Contains(c.UserId))
                    .GroupBy(c => c.UserId)
                    .Select(g => new { UserId = g.Key, TotalOrders = g.Count() })
                    .ToListAsync();

                var items = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((filter.PageIndex - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(u => new UserListItemDto
                    {
                        UserId = u.Id,
                        FullName = u.FullName ?? string.Empty,
                        Email = u.Email ?? string.Empty,
                        PhoneNumber = u.PhoneNumber ?? "",
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        // Lấy tổng số đơn hàng của khách hàng từ danh sách customerOrders
                        // Vẫn chưa test, có thể lỗi khi chạy chương trình.
                        TotalOrders = customerOrders.Any(co => co.UserId == u.Id) ? customerOrders.First(co => co.UserId == u.Id).TotalOrders : 0
                    })
                    .ToListAsync();

                var result = new PagedResultDto<UserListItemDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize
                };

                return ApiResponse<PagedResultDto<UserListItemDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResultDto<UserListItemDto>>.FailureResult($"Lỗi lấy danh sách khách hàng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<UserListItemDto>>> GetAllStaffAsync()
        {
            try
            {
                var staffUserIds = await (from ur in _context.UserRoles
                                          join r in _roleManager.Roles on ur.RoleId equals r.Id
                                          where r.Name != "Customer" && r.Name != "Admin"
                                          select ur.UserId)
                                          .Distinct()
                                          .ToListAsync();

                var staffUsers = await _userManager.Users
                    .AsNoTracking()
                    .Where(u => staffUserIds.Contains(u.Id))
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                var staffList = new List<UserListItemDto>();

                foreach (var user in staffUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    staffList.Add(new UserListItemDto
                    {
                        UserId = user.Id,
                        FullName = user.FullName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        PhoneNumber = user.PhoneNumber ?? "",
                        AvatarUrl = user.AvatarUrl ?? string.Empty,
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt,
                        Roles = roles.ToList()
                    });
                }

                return ApiResponse<List<UserListItemDto>>.SuccessResult(staffList);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UserListItemDto>>.FailureResult($"Lỗi lấy danh sách nhân viên: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CreateStaffAsync(CreateEmployeeDto dto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email!);
                if (existingUser != null)
                {
                    return ApiResponse<bool>.FailureResult("Email này đã được sử dụng trong hệ thống.");
                }

                var user = User.CreateCustomer(dto.Email!, dto.FullName!, dto.PhoneNumber);

                var createResult = await _userManager.CreateAsync(user, dto.Password!);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return ApiResponse<bool>.FailureResult($"Không thể tạo tài khoản: {errors}");
                }

                if (!await _roleManager.RoleExistsAsync(dto.Role!))
                {
                    await _roleManager.CreateAsync(new Role { Name = dto.Role! });
                }

                await _userManager.AddToRoleAsync(user, dto.Role!);
                return ApiResponse<bool>.SuccessResult(true, "Tạo nhân viên thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi tạo nhân viên: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStaffRoleAsync(UpdateEmployeeDto dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(dto.UserId!);
                if (user == null)
                {
                    return ApiResponse<bool>.FailureResult("Không tìm thấy thông tin nhân viên.");
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!await _roleManager.RoleExistsAsync(dto.Role!))
                {
                    await _roleManager.CreateAsync(new Role { Name = dto.Role! });
                }

                await _userManager.AddToRoleAsync(user, dto.Role!);
                return ApiResponse<bool>.SuccessResult(true, "Cập nhật vai trò thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi cập nhật vai trò: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> BanUserAsync(BanUserDto dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(dto.UserId!);
                if (user == null)
                {
                    return ApiResponse<bool>.FailureResult("Không tìm thấy người dùng.");
                }

                user.Deactivate();
                //user.BanReason = dto.Reason;
                //user.BannedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded
                    ? ApiResponse<bool>.SuccessResult(true, "Khóa tài khoản thành công.")
                    : ApiResponse<bool>.FailureResult("Khóa tài khoản thất bại.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi khóa tài khoản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UnbanUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.FailureResult("Không tìm thấy người dùng.");
                }

                user.Activate();
                //user.BanReason = null;
                //user.BannedAt = null;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded
                    ? ApiResponse<bool>.SuccessResult(true, "Mở khóa tài khoản thành công.")
                    : ApiResponse<bool>.FailureResult("Mở khóa tài khoản thất bại.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi mở khóa tài khoản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserListItemDto>> GetCustomerDetailAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return ApiResponse<UserListItemDto>.FailureResult("Không tìm thấy khách hàng.");

                var roles = await _userManager.GetRolesAsync(user);
                var totalOrders = await _context.Set<Order>().CountAsync(o => o.UserId == userId);

                var dto = new UserListItemDto
                {
                    UserId = user.Id,
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? "",
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    TotalOrders = totalOrders,
                    Roles = roles.ToList()
                };

                return ApiResponse<UserListItemDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserListItemDto>.FailureResult($"Lỗi khi lấy chi tiết khách hàng: {ex.Message}");
            }
        }
    }
}