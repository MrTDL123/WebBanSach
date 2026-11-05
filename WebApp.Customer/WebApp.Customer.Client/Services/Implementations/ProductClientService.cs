using Microsoft.AspNetCore.Components.Forms;
using WebApp.Customer.Client.Services.Base;
using WebApp.Customer.Client.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Product;
using WebApp.Shared.Dtos.CustomerDtos.Review;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Customer.Client.Services.Implementations
{
    public class ProductClientService : BaseApiClient, IProductClientService
    {
        public ProductClientService(
            HttpClient httpClient,
            ICookieProvider? cookieProvider = null,
            AntiforgeryStateProvider? antiforgeryStateProvider = null)
            : base(httpClient, cookieProvider, antiforgeryStateProvider)
        {
        }

        public async Task<ApiResponse<HomePageDataDto>> GetHomePageDataAsync()
        {
            return await GetAsync<HomePageDataDto>("api/products/home");
        }

        public async Task<ApiResponse<List<ProductListItemDto>>> GetFeaturedProductsAsync(int count = 8)
        {
            return await GetAsync<List<ProductListItemDto>>($"api/products/featured?count={count}");
        }

        public async Task<ApiResponse<List<ProductListItemDto>>> GetProductsAsync(WebApp.Shared.Dtos.CustomerDtos.Product.ProductFilterDto filter)
        {
            var paged = await GetPagedProductsAsync(filter);
            return ApiResponse<List<ProductListItemDto>>.SuccessResult(paged.Data?.Items ?? new List<ProductListItemDto>());
        }

        public async Task<ApiResponse<PagedResult<ProductListItemDto>>> GetPagedProductsAsync(WebApp.Shared.Dtos.CustomerDtos.Product.ProductFilterDto filter)
        {
            var queryString = BuildFilterQueryString(filter);
            return await GetAsync<PagedResult<ProductListItemDto>>($"api/products{queryString}");
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync()
        {
            return await GetAsync<List<CategoryDto>>("api/categories");
        }

        public async Task<ApiResponse<List<AuthorDto>>> GetAuthorsAsync()
        {
            return await GetAsync<List<AuthorDto>>("api/categories/authors");
        }

        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
        {
            return await GetAsync<ProductDto>($"api/products/{id}");
        }

        public async Task<ApiResponse<PagedResult<ProductReviewDto>>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 10)
        {
            return await GetAsync<PagedResult<ProductReviewDto>>($"api/products/{productId}/reviews?page={page}&pageSize={pageSize}");
        }

        public async Task<ApiResponse<ProductReviewDto>> CreateReviewAsync(CreateReviewDto dto)
        {
            return await PostAsJsonAsync<CreateReviewDto, ProductReviewDto>("api/products/reviews", dto);
        }

        private static string BuildFilterQueryString(WebApp.Shared.Dtos.CustomerDtos.Product.ProductFilterDto filter)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                queryParams.Add($"keyword={Uri.EscapeDataString(filter.Keyword)}");
            if (filter.CategoryId.HasValue)
                queryParams.Add($"categoryId={filter.CategoryId.Value}");
            if (filter.AuthorId.HasValue)
                queryParams.Add($"authorId={filter.AuthorId.Value}");
            if (filter.PublisherId.HasValue)
                queryParams.Add($"publisherId={filter.PublisherId.Value}");
            if (filter.MinPrice.HasValue)
                queryParams.Add($"minPrice={filter.MinPrice.Value}");
            if (filter.MaxPrice.HasValue)
                queryParams.Add($"maxPrice={filter.MaxPrice.Value}");
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
                queryParams.Add($"sortBy={Uri.EscapeDataString(filter.SortBy)}");
            queryParams.Add($"page={filter.Page}");
            queryParams.Add($"pageSize={filter.PageSize}");

            return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
        }
    }
}
