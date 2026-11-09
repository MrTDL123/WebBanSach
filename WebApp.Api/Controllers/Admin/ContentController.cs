using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IValidator<CreateProductDto> _createProductValidator;
        private readonly IValidator<CreateCategoryDto> _createCategoryValidator;
        private readonly IValidator<CreateAuthorDto> _createAuthorValidator;
        private readonly IValidator<CreatePublisherDto> _createPublisherValidator;
        private readonly IValidator<CreateVoucherDto> _createVoucherValidator;
        private readonly IValidator<CreateFreeBookDto> _createFreeBookValidator;
        private readonly IValidator<UpdateProductDto> _updateProductValidator;
        private readonly IValidator<UpdateCategoryDto> _updateCategoryValidator;
        private readonly IValidator<UpdateAuthorDto> _updateAuthorValidator;
        private readonly IValidator<UpdatePublisherDto> _updatePublisherValidator;
        private readonly IValidator<UpdateVoucherDto> _updateVoucherValidator;
        private readonly IValidator<UpdateFreeBookDto> _updateFreeBookValidator;
        public ContentController(
            IContentService contentService, 
            IValidator<CreateProductDto> createProductValidator,
            IValidator<CreateCategoryDto> createCategoryValidator,
            IValidator<CreateAuthorDto> createAuthorValidator,
            IValidator<CreatePublisherDto> createPublisherValidator,
            IValidator<CreateVoucherDto> createVoucherValidator,
            IValidator<CreateFreeBookDto> createFreeBookValidator,
            IValidator<UpdateProductDto> updateProductValidator,
            IValidator<UpdateCategoryDto> updateCategoryValidator,
            IValidator<UpdateAuthorDto> updateAuthorValidator,
            IValidator<UpdatePublisherDto> updatePublisherValidator,
            IValidator<UpdateVoucherDto> updateVoucherValidator,
            IValidator<UpdateFreeBookDto> updateFreeBookValidator
        )
        {
            _contentService = contentService;
            _createProductValidator = createProductValidator;
            _updateProductValidator = updateProductValidator;
            _createCategoryValidator = createCategoryValidator;
            _updateCategoryValidator = updateCategoryValidator;
            _createAuthorValidator = createAuthorValidator;
            _updateAuthorValidator = updateAuthorValidator;
            _createPublisherValidator = createPublisherValidator;
            _updatePublisherValidator = updatePublisherValidator;
            _createVoucherValidator = createVoucherValidator;
            _updateVoucherValidator = updateVoucherValidator;
            _createFreeBookValidator = createFreeBookValidator;
            _updateFreeBookValidator = updateFreeBookValidator;
        }

        /// <summary>
        /// Quản lý sản phẩm: Lấy danh sách sản phẩm với phân trang và lọc, lấy thông tin chi tiết, xóa, tạo mới và cập nhật sản phẩm.
        /// </summary>
        [HttpGet("products")]
        public async Task<ActionResult<ApiResponse<PagedResultDto<ProductListDto>>>> GetProducts([FromQuery] ProductFilterDto filterDto)
        {
            var response = await _contentService.GetProductsAsync(filterDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("products/{id:int}")]
        public async Task<ActionResult<ApiResponse<UpdateProductDto>>> GetProductForUpdate([FromRoute] int id)
        {
            var response = await _contentService.GetProductForUpdateAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("products")]
        public async Task<ActionResult<ApiResponse<bool>>> CreateProduct([FromBody] CreateProductDto dto)
        {
            var validationResult = await _createProductValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }

            var response = await _contentService.CreateProductAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("products")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            var validationResult = await _updateProductValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }

            var response = await _contentService.UpdateProductAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("products/{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct([FromRoute] int id)
        {
            var response = await _contentService.DeleteProductAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet("freebooks")]
        public async Task<ActionResult<ApiResponse<PagedResultDto<FreeBookListDto>>>> GetFreeBooks([FromQuery] FreeBookFilterDto filterDto)
        {
            var response = await _contentService.GetFreeBooksAsync(filterDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("freebooks/{id:int}")]
        public async Task<ActionResult<ApiResponse<UpdateFreeBookDto>>> GetFreeBookForUpdate([FromRoute] int id)
        {
            var response = await _contentService.GetFreeBookForUpdateAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("freebooks/{id:int}/detail")]
        public async Task<ActionResult<ApiResponse<FreeBookDetailDto>>> GetFreeBookDetail([FromRoute] int id)
        {
            var response = await _contentService.GetFreeBookDetailAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("freebooks")]
        public async Task<ActionResult<ApiResponse<bool>>> CreateFreeBook([FromBody] CreateFreeBookDto dto)
        {
            var validationResult = await _createFreeBookValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }

            var response = await _contentService.CreateFreeBookAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("freebooks")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateFreeBook([FromBody] UpdateFreeBookDto dto)
        {
            var validationResult = await _updateFreeBookValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }

            var response = await _contentService.UpdateFreeBookAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("freebooks/{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteFreeBook([FromRoute] int id)
        {
            var response = await _contentService.DeleteFreeBookAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Quản lý danh mục: Lấy danh sách, tạo mới, cập nhật và xóa.
        /// </summary>
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

        [HttpPost("categories")]
        public async Task<ActionResult<ApiResponse<bool>>> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var validationResult = await _createCategoryValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }
            var response = await _contentService.CreateCategoryAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("categories")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCategory([FromBody] UpdateCategoryDto dto)
        {
            var validationResult = await _updateCategoryValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }
            var response = await _contentService.UpdateCategoryAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("categories/{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory([FromRoute] int id)
        {
            var response = await _contentService.DeleteCategoryAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Quản lý tác giả: Tạo mới, cập nhật và xóa.
        /// </summary>
        [HttpGet("authors")]
        public async Task<ActionResult<ApiResponse<List<AuthorDto>>>> GetAuthors()
        {
            var response = await _contentService.GetAuthorsAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("authors")]
        public async Task<ActionResult<ApiResponse<bool>>> CreateAuthor([FromBody] CreateAuthorDto dto)
        {
            var validationResult = await _createAuthorValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }
            var response = await _contentService.CreateAuthorAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("authors")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateAuthor([FromBody] UpdateAuthorDto dto)
        {
            var validationResult = await _updateAuthorValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }
            var response = await _contentService.UpdateAuthorAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("authors/{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAuthor([FromRoute] int id)
        {
            var response = await _contentService.DeleteAuthorAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Quản lý nhà xuất bản: Tạo mới, cập nhật và xóa.
        /// </summary>
        [HttpGet("publishers")]
        public async Task<ActionResult<ApiResponse<List<PublisherDto>>>> GetPublishers()
        {
            var response = await _contentService.GetPublishersAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("publishers")]
        public async Task<ActionResult<ApiResponse<bool>>> CreatePublisher([FromBody] CreatePublisherDto dto)
        {
            var validationResult = await _createPublisherValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }
            var response = await _contentService.CreatePublisherAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("publishers")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePublisher([FromBody] UpdatePublisherDto dto)
        {
            var validationResult = await _updatePublisherValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }
            var response = await _contentService.UpdatePublisherAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("publishers/{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePublisher([FromRoute] int id)
        {
            var response = await _contentService.DeletePublisherAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Quản lý voucher: Tạo mới, cập nhật và xóa.
        /// </summary>
        [HttpGet("vouchers")]
        public async Task<ActionResult<ApiResponse<List<VoucherDto>>>> GetVouchers()
        {
            var response = await _contentService.GetVouchersAsync();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("vouchers")]
        public async Task<ActionResult<ApiResponse<bool>>> CreateVoucher([FromBody] CreateVoucherDto dto)
        {
            var validationResult = await _createVoucherValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }
            var response = await _contentService.CreateVoucherAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("vouchers")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateVoucher([FromBody] UpdateVoucherDto dto)
        {
            var validationResult = await _updateVoucherValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.FailureResult("Dữ liệu không hợp lệ", errors));
            }
            var response = await _contentService.UpdateVoucherAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("vouchers/{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteVoucher([FromRoute] int id)
        {
            var response = await _contentService.DeleteVoucherAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
