using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Services.Implementations;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.Admin;

namespace WebApp.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IValidator<CreateEmployeeDto> _createEmployeeValidator;
        private readonly IValidator<UpdateEmployeeDto> _updateEmployeeValidator;
        private readonly IValidator<BanUserDto> _banUserValidator;

        public AdminController(
            IAdminService adminService, 
            IValidator<CreateEmployeeDto> createEmployeeValidator, 
            IValidator<UpdateEmployeeDto> updateEmployeeValidator, 
            IValidator<BanUserDto> banUserValidator
        )
        {
            _adminService = adminService;
            _createEmployeeValidator = createEmployeeValidator;
            _updateEmployeeValidator = updateEmployeeValidator;
            _banUserValidator = banUserValidator;
        }

        [HttpGet("customers")]
        public async Task<ActionResult<ApiResponse<PagedResultDto<UserListItemDto>>>> GetCustomersPaged([FromQuery] UserFilterDto filter)
        {
            var response = await _adminService.GetCustomersPagedAsync(filter);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("customers/{id}")]
        public async Task<ActionResult<ApiResponse<UserListItemDto>>> GetCustomerDetail([FromRoute] string id)
        {
            var response = await _adminService.GetCustomerDetailAsync(id);
            if (!response.Success)
            {
                    
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("staffs")]
        public async Task<IActionResult> GetAllStaff()
        {
            var response = await _adminService.GetAllStaffAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("staffs")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateEmployeeDto dto)
        {
            var validationResult = await _createEmployeeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }

            var response = await _adminService.CreateStaffAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("staffs/roles")]
        public async Task<IActionResult> UpdateStaffRole([FromBody] UpdateEmployeeDto dto)
        {
            var validationResult = await _updateEmployeeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }

            var response = await _adminService.UpdateStaffRoleAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("users/ban")]
        public async Task<IActionResult> BanUser([FromBody] BanUserDto dto)
        {
            var validationResult = await _banUserValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }

            var response = await _adminService.BanUserAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("users/unban/{id}")]
        public async Task<IActionResult> UnbanUser([FromRoute] string id)
        {
            var response = await _adminService.UnbanUserAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
