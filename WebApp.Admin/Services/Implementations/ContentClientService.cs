using Microsoft.AspNetCore.WebUtilities;
using WebApp.Admin.Services.Base;
using WebApp.Admin.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;
using static System.Net.WebRequestMethods;

namespace WebApp.Admin.Services.Implementations
{
    public class ContentClientService : BaseApiClient, IContentClientService
    {
        private readonly HttpClient _httpClient;

        public ContentClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // <summary>
        // 
        // </summary>
        public Task<ApiResponse<List<ProductListDto>>> GetProductsAsync(ProductFilterDto filterDto)
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
            return ExecuteApiAsync<List<ProductListDto>>(() => _httpClient.GetAsync(url));
        }

        public Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync()
        {
            return ExecuteApiAsync<List<CategoryDto>>(() => _httpClient.GetAsync("api/management/content/categories"));
        }
    }
}
