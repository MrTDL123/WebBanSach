using Microsoft.AspNetCore.WebUtilities;
using WebApp.Admin.Services.Base;
using WebApp.Admin.Services.Interfaces;
using WebApp.Admin.Utilities;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Services.Implementations
{
    public class ContentClientService : BaseApiClient, IContentClientService
    {
        public ContentClientService(
            HttpClient httpClient,
            UserSessionState userSessionState,
            IHttpContextAccessor httpContextAccessor)
            : base(httpClient, userSessionState, httpContextAccessor)
        {
        }

        /// <summary>
        /// Lấy danh sách sản phẩm với phân trang và lọc.
        /// </summary>
        public Task<ApiResponse<PagedResultDto<ProductListDto>>> GetProductsAsync(ProductFilterDto filterDto)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["searchTerm"] = filterDto.SearchTerm,
                ["categoryId"] = filterDto.CategoryId?.ToString(),
                ["isActive"] = filterDto.IsActive?.ToString(),
                ["isLowStockOnly"] = filterDto.IsLowStockOnly.ToString(),
                ["pageIndex"] = filterDto.PageIndex.ToString(),
                ["pageSize"] = filterDto.PageSize.ToString()
            };

            // 2. Dùng 'QueryHelpers.AddQueryString' của Microsoft để ghép URL thay vì cộng chuỗi thủ công vì:
            //    - Tự động LOẠI BỎ các tham số có giá trị null (giúp URL sạch sẽ, không bị dính param rác).
            //    - Tự động MÃ HÓA ký tự đặc biệt (URL Encoding) như tiếng Việt có dấu, khoảng trắng, dấu '&'.
            //    - Tự động gắn chuẩn xác dấu '?' ở đầu và các dấu '&' nối giữa các tham số.
            string url = QueryHelpers.AddQueryString("api/management/content/products", queryParams);
            return GetAsync<PagedResultDto<ProductListDto>>(url);
        }

        /// <summary>
        /// Quản lý sản phẩm: Lấy thông tin chi tiết, xóa, tạo mới và cập nhật sản phẩm.
        /// </summary>
        public Task<ApiResponse<UpdateProductDto>> GetProductForUpdateAsync(int id)
        {
            return GetAsync<UpdateProductDto>($"api/management/content/products/{id}");
        }

        public Task<ApiResponse<bool>> CreateProductAsync(CreateProductDto dto)
        {
            return PostAsJsonAsync<CreateProductDto, bool>("api/management/content/products", dto);
        }

        public Task<ApiResponse<bool>> UpdateProductAsync(UpdateProductDto dto)
        {
            return PutAsJsonAsync<UpdateProductDto, bool>("api/management/content/products", dto);
        }

        public Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            return DeleteAsync<bool>($"api/management/content/products/{id}");
        }

        /// <summary>
        /// Lấy danh sách sách tủ sách 0 đồng với phân trang và lọc.
        /// </summary> 
        public Task<ApiResponse<PagedResultDto<FreeBookListDto>>> GetFreeBooksAsync(FreeBookFilterDto filterDto)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["searchTerm"] = filterDto.SearchTerm,
                ["isActive"] = filterDto.IsActive?.ToString(),
                ["stockStatus"] = filterDto.StockStatus?.ToString(),
                ["sortBy"] = filterDto.SortBy?.ToString(),
                ["pageIndex"] = filterDto.PageIndex.ToString(),
                ["pageSize"] = filterDto.PageSize.ToString()
            };

            string url = QueryHelpers.AddQueryString("api/management/content/freebooks", queryParams);
            return GetAsync<PagedResultDto<FreeBookListDto>>(url);
        }

        /// <summary>
        /// Quản lý sách tủ sách 0 đồng: Lấy thông tin chi tiết, xóa, tạo mới và cập nhật sách tủ sách 0 đồng.
        /// </summary>
        public Task<ApiResponse<UpdateFreeBookDto>> GetFreeBookForUpdateAsync(int id)
        {
            return GetAsync<UpdateFreeBookDto>($"api/management/content/freebooks/{id}");
        }

        public Task<ApiResponse<FreeBookDetailDto>> GetFreeBookDetailAsync(int id)
        {
            return GetAsync<FreeBookDetailDto>($"api/management/content/freebooks/{id}/detail");
        }

        public Task<ApiResponse<bool>> CreateFreeBookAsync(CreateFreeBookDto dto)
        {
            return PostAsJsonAsync<CreateFreeBookDto, bool>("api/management/content/freebooks", dto);
        }

        public Task<ApiResponse<bool>> UpdateFreeBookAsync(UpdateFreeBookDto dto)
        {
            return PutAsJsonAsync<UpdateFreeBookDto, bool>("api/management/content/freebooks", dto);
        }

        public Task<ApiResponse<bool>> DeleteFreeBookAsync(int id)
        {
            return DeleteAsync<bool>($"api/management/content/freebooks/{id}");
        }

        /// <summary>
        /// Quản lý danh mục, tác giả, nhà xuất bản và voucher: Lấy danh sách, tạo mới, cập nhật và xóa.
        /// </summary>
        public Task<ApiResponse<bool>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            return PostAsJsonAsync<CreateCategoryDto, bool>("api/management/content/categories", dto);
        }

        public Task<ApiResponse<bool>> CreateAuthorAsync(CreateAuthorDto dto)
        {
            return PostAsJsonAsync<CreateAuthorDto, bool>("api/management/content/authors", dto);
        }

        public Task<ApiResponse<bool>> CreatePublisherAsync(CreatePublisherDto dto)
        {
            return PostAsJsonAsync<CreatePublisherDto, bool>("api/management/content/publishers", dto);
        }

        public Task<ApiResponse<bool>> CreateVoucherAsync(CreateVoucherDto dto)
        {
            return PostAsJsonAsync<CreateVoucherDto, bool>("api/management/content/vouchers", dto);
        }

        public Task<ApiResponse<bool>> UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            return PutAsJsonAsync<UpdateCategoryDto, bool>("api/management/content/categories", dto);
        }

        public Task<ApiResponse<bool>> UpdateAuthorAsync(UpdateAuthorDto dto)
        {
            return PutAsJsonAsync<UpdateAuthorDto, bool>("api/management/content/authors", dto);
        }

        public Task<ApiResponse<bool>> UpdatePublisherAsync(UpdatePublisherDto dto)
        {
            return PutAsJsonAsync<UpdatePublisherDto, bool>("api/management/content/publishers", dto);
        }

        public Task<ApiResponse<bool>> UpdateVoucherAsync(UpdateVoucherDto dto)
        {
            return PutAsJsonAsync<UpdateVoucherDto, bool>("api/management/content/vouchers", dto);
        }

        public Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
        {
            return DeleteAsync<bool>($"api/management/content/categories/{id}");
        }

        public Task<ApiResponse<bool>> DeleteAuthorAsync(int id)
        {
            return DeleteAsync<bool>($"api/management/content/authors/{id}");
        }

        public Task<ApiResponse<bool>> DeletePublisherAsync(int id)
        {
            return DeleteAsync<bool>($"api/management/content/publishers/{id}");
        }

        public Task<ApiResponse<bool>> DeleteVoucherAsync(int id)
        {
            return DeleteAsync<bool>($"api/management/content/vouchers/{id}");
        }

        /// <summary>
        /// Lấy danh sách các tác giả, nhà xuất bản, danh mục và voucher.
        /// </summary>
        public Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync()
        {
            return GetAsync<List<CategoryDto>>("api/management/content/categories");
        }

        public Task<ApiResponse<List<AuthorDto>>> GetAuthorsAsync()
        {
            return GetAsync<List<AuthorDto>>("api/management/content/authors");
        }

        public Task<ApiResponse<List<PublisherDto>>> GetPublishersAsync()
        {
            return GetAsync<List<PublisherDto>>("api/management/content/publishers");
        }

        public Task<ApiResponse<List<VoucherDto>>> GetVouchersAsync()
        {
            return GetAsync<List<VoucherDto>>("api/management/content/vouchers");
        }
    }
}
