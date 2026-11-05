using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Product;
using WebApp.Shared.Dtos.CustomerDtos.Review;
using AuthorDto = WebApp.Shared.Dtos.Management.ContentAndMarketing.AuthorDto;
using CategoryDto = WebApp.Shared.Dtos.Management.ContentAndMarketing.CategoryDto;
using CustomerProductFilterDto = WebApp.Shared.Dtos.CustomerDtos.Product.ProductFilterDto;

namespace WebApp.Api.Services.Interfaces
{
    public interface ICustomerProductService
    {
        Task<ApiResponse<PagedResult<ProductListItemDto>>> GetProductsAsync(CustomerProductFilterDto filter);
        Task<ApiResponse<List<ProductListItemDto>>> GetFeaturedProductsAsync(int count = 8);
        Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync();

        // Mở rộng phục vụ Home & Review
        Task<ApiResponse<HomePageDataDto>> GetHomePageDataAsync();
        Task<ApiResponse<List<AuthorDto>>> GetAuthorsAsync();
        Task<ApiResponse<PagedResult<ProductReviewDto>>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 10, string? currentUserId = null);
        Task<ApiResponse<ProductReviewDto>> CreateProductReviewAsync(string userId, CreateReviewDto dto);
    }
}
