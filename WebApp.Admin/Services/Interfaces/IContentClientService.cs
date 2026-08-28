using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Admin.Services.Interfaces
{
    public interface IContentClientService
    {
        Task<ApiResponse<List<ProductListDto>>> GetProductsAsync(ProductFilterDto filterDto);
        Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync();
    }
}
