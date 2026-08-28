using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/management/[controller]")]
    [Authorize(Roles = "Admin,ContentManager")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;
        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet("products")]
        public async Task<ActionResult<ApiResponse<List<ProductListDto>>>> GetProducts([FromQuery] ProductFilterDto filterDto)
        {
            var response = await _contentService.GetProductsAsync(filterDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategories()
        {
            var response = await _contentService.GetCategoriesAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
