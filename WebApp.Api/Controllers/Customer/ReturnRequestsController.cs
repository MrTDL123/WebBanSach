using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Order;

namespace WebApp.Api.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReturnRequestsController : ControllerBase
    {
        private readonly ICustomerReturnService _returnService;
        private readonly IValidator<CreateReturnRequestDto> _createValidator;

        public ReturnRequestsController(
            ICustomerReturnService returnService,
            IValidator<CreateReturnRequestDto> createValidator)
        {
            _returnService = returnService;
            _createValidator = createValidator;
        }

        /// <summary>
        /// Tạo yêu cầu đổi trả hàng mới (Endpoint #8 - Thống nhất cho cả Web và Mobile)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReturnRequestSummaryDto>>> CreateReturnRequest([FromBody] CreateReturnRequestDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ReturnRequestSummaryDto>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<ReturnRequestSummaryDto>.FailureResult("Xác thực không hợp lệ."));
            }

            var response = await _returnService.CreateReturnRequestAsync(userId, dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách các phiếu yêu cầu trả hàng của người dùng hiện tại (Endpoint #9)
        /// </summary>
        [HttpGet("my-returns")]
        public async Task<ActionResult<ApiResponse<List<ReturnRequestSummaryDto>>>> GetMyReturns()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<List<ReturnRequestSummaryDto>>.FailureResult("Xác thực không hợp lệ."));
            }

            var response = await _returnService.GetMyReturnRequestsAsync(userId);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
