using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Product;
using WebApp.Shared.Dtos.CustomerDtos.Review;

namespace WebApp.Api.Controllers.Customer
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ICustomerProductService _productService;
        private readonly IValidator<CreateReviewDto> _reviewValidator;

        public ProductController(
            ICustomerProductService productService,
            IValidator<CreateReviewDto> reviewValidator)
        {
            _productService = productService;
            _reviewValidator = reviewValidator;
        }

        [HttpGet("home")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<HomePageDataDto>>> GetHomePageData()
        {
            var result = await _productService.GetHomePageDataAsync();
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductListItemDto>>>> GetProducts([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetProductsAsync(filter);
            return Ok(result);
        }

        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ProductListItemDto>>>> GetFeatured([FromQuery] int count = 8)
        {
            var result = await _productService.GetFeaturedProductsAsync(count);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("{id:int}/reviews")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductReviewDto>>>> GetReviews(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _productService.GetProductReviewsAsync(id, page, pageSize, currentUserId);
            return Ok(result);
        }

        [HttpPost("reviews")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductReviewDto>>> CreateReview([FromBody] CreateReviewDto dto)
        {
            var validationResult = await _reviewValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ProductReviewDto>.FailureResult("Dữ liệu không hợp lệ.", errors));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<ProductReviewDto>.FailureResult("Xác thực không hợp lệ."));
            }

            var result = await _productService.CreateProductReviewAsync(userId, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
