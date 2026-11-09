using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Api.Controllers.Customer
{
    [ApiController]
    [Route("api/categories")]
    [AllowAnonymous]
    public class CategoryController : ControllerBase
    {
        private readonly ICustomerProductService _productService;

        public CategoryController(ICustomerProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategories()
        {
            var result = await _productService.GetCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("authors")]
        public async Task<ActionResult<ApiResponse<List<AuthorDto>>>> GetAuthors()
        {
            var result = await _productService.GetAuthorsAsync();
            return Ok(result);
        }
    }
}
