using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Order;

namespace WebApp.Api.Controllers.Customer
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ICustomerOrderService _orderService;
        private readonly IValidator<CreateOrderDto> _createOrderValidator;

        public OrdersController(
            ICustomerOrderService orderService,
            IValidator<CreateOrderDto> createOrderValidator)
        {
            _orderService = orderService;
            _createOrderValidator = createOrderValidator;
        }

        [HttpPost("apply-voucher")]
        public async Task<ActionResult<ApiResponse<VoucherApplyResultDto>>> ApplyVoucher([FromBody] ApplyVoucherDto dto)
        {
            var result = await _orderService.ApplyVoucherAsync(dto);
            return Ok(result);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> Checkout([FromBody] CreateOrderDto dto)
        {
            var validationResult = await _createOrderValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<OrderDto>.FailureResult("Dữ liệu đặt hàng không hợp lệ.", errors));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<OrderDto>.FailureResult("Xác thực không hợp lệ."));
            }

            var result = await _orderService.CheckoutAsync(userId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<ApiResponse<List<OrderListItemDto>>>> GetMyOrders([FromQuery] string? status = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<List<OrderListItemDto>>.FailureResult("Xác thực không hợp lệ."));
            }

            var result = await _orderService.GetMyOrdersAsync(userId, status);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<OrderDto>.FailureResult("Xác thực không hợp lệ."));
            }

            var result = await _orderService.GetOrderByIdAsync(userId, id);
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(int id, [FromBody] CancelOrderRequestDto? dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.FailureResult("Xác thực không hợp lệ."));
            }

            var result = await _orderService.CancelOrderAsync(userId, id, dto?.CancelReason);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
