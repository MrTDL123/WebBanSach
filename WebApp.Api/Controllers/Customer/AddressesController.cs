using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Address;

namespace WebApp.Api.Controllers.Customer
{
    [ApiController]
    [Route("api/addresses")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly ICustomerAddressService _addressService;
        private readonly IValidator<CreateAddressDto> _createValidator;
        private readonly IValidator<UpdateAddressDto> _updateValidator;

        public AddressesController(
            ICustomerAddressService addressService,
            IValidator<CreateAddressDto> createValidator,
            IValidator<UpdateAddressDto> updateValidator)
        {
            _addressService = addressService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AddressDto>>>> GetMyAddresses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<List<AddressDto>>.FailureResult("Xác thực không hợp lệ."));
            }

            var result = await _addressService.GetMyAddressesAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AddressDto>>> CreateAddress([FromBody] CreateAddressDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AddressDto>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<AddressDto>.FailureResult("Xác thực không hợp lệ."));
            }

            var result = await _addressService.CreateAddressAsync(userId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<AddressDto>>> UpdateAddress(int id, [FromBody] UpdateAddressDto dto)
        {
            var updateDto = dto with { AddressId = id };
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AddressDto>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<AddressDto>.FailureResult("Xác thực không hợp lệ."));
            }

            var result = await _addressService.UpdateAddressAsync(userId, updateDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAddress(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.FailureResult("Xác thực không hợp lệ."));
            }

            var result = await _addressService.DeleteAddressAsync(userId, id);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
