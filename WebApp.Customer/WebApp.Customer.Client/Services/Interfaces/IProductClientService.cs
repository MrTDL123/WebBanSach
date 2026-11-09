using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Product;
using WebApp.Shared.Dtos.CustomerDtos.Review;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Customer.Client.Services.Interfaces
{
    public interface IProductClientService
    {
        Task<ApiResponse<HomePageDataDto>> GetHomePageDataAsync();
        Task<ApiResponse<List<ProductListItemDto>>> GetFeaturedProductsAsync(int count = 8);
        Task<ApiResponse<List<ProductListItemDto>>> GetProductsAsync(WebApp.Shared.Dtos.CustomerDtos.Product.ProductFilterDto filter);
        Task<ApiResponse<PagedResult<ProductListItemDto>>> GetPagedProductsAsync(WebApp.Shared.Dtos.CustomerDtos.Product.ProductFilterDto filter);
        Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync();
        Task<ApiResponse<List<AuthorDto>>> GetAuthorsAsync();
        Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<PagedResult<ProductReviewDto>>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 10);
        Task<ApiResponse<ProductReviewDto>> CreateReviewAsync(CreateReviewDto dto);
    }
}
