using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Api.Services.Interfaces
{
    public interface IContentService
    {
        Task<ApiResponse<List<ProductListDto>>> GetProductsAsync(ProductFilterDto filterDto);
        Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync();
    }
}
