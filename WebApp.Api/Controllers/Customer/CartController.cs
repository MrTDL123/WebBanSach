using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Cart;

namespace WebApp.Api.Controllers.Customer
{
    [ApiController]
    [Route("api/cart")]
    [AllowAnonymous]
    public class CartController : ControllerBase
    {
        private readonly ICustomerCartService _cartService;

        public CartController(ICustomerCartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<CartDto>>> GetCart()
        {
            var result = await _cartService.GetCartAsync();
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse<CartDto>>> AddToCart([FromBody] AddToCartDto dto)
        {
            var result = await _cartService.AddToCartAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("items")]
        public async Task<ActionResult<ApiResponse<CartDto>>> UpdateCartItem([FromBody] UpdateCartItemDto dto)
        {
            var result = await _cartService.UpdateCartItemAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("items/{productId:int}")]
        public async Task<ActionResult<ApiResponse<CartDto>>> UpdateCartItemByProductId(int productId, [FromBody] UpdateCartItemQuantityDto dto)
        {
            var result = await _cartService.UpdateCartItemAsync(new UpdateCartItemDto(productId, dto.Quantity));
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("items/{productId:int}")]
        public async Task<ActionResult<ApiResponse<CartDto>>> RemoveCartItem(int productId)
        {
            var result = await _cartService.RemoveCartItemAsync(productId);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<bool>>> ClearCart()
        {
            var result = await _cartService.ClearCartAsync();
            return Ok(result);
        }
    }
}
