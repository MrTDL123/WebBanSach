using Microsoft.EntityFrameworkCore;
using WebApp.Api.Data;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Api.Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly AppDbContext _context;

        public ContentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<ProductListDto>>> GetProductsAsync(ProductFilterDto filterDto)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
            {
                var term = filterDto.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(term)
                                        || (p.Author != null && p.Author.AuthorName.ToLower().Contains(term)));
            }

            // Vì trong ProductFilterDto CategoryId được khai báo kiểu dữ liệu int? (Nullable int) có thuộc
            //  tính .HashValue để kiểm tra xem biến này có chứa số hay đang bị null.
            // 👉 Nếu người dùng CÓ CHỌN danh mục (HasValue == true):
            // Thêm điều kiện lọc vào câu lệnh SQL: WHERE CategoryId = 1
            // 👉 Nếu người dùng chọn "Tất cả danh mục" (HasValue == false / null):
            // Bỏ qua, KHÔNG lọc gì cả ➔ Lấy toàn bộ sách của tất cả danh mục lên!
            if (filterDto.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId ==  filterDto.CategoryId.Value);
            }

            // Vì trong ProductFilterDto CategoryId được khai báo kiểu dữ liệu bool? (Nullable bool) có thuộc
            //  tính .HashValue để kiểm tra xem biến này có chứa giá trị (true hoặc false) hay đang bị null.
            // Tương tự như ở trên categoryid
            if (filterDto.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == filterDto.IsActive.Value);
            }

            if (filterDto.IsLowStockOnly)
            {
                query = query.Where(p => p.StockQuantity < 10);
            }

            var items = await query
                .OrderByDescending(p => p.ProductId)
                .Skip((filterDto.PageIndex - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .Select(p => new ProductListDto(
                    p.ProductId,
                    p.Title,
                    p.MainImageUrl,
                    p.Category != null ? p.Category.CategoryName : string.Empty,
                    p.Author != null ? p.Author.AuthorName : string.Empty,
                    p.Publisher != null ? p.Publisher.PublisherName : string.Empty,
                    p.Price,
                    p.DiscountPercent,
                    p.DiscountPercent > 0 ? p.Price * (100 - p.DiscountPercent) / 100 : p.Price,
                    p.StockQuantity,
                    p.IsActive,
                    p.CreatedAt
                )).ToListAsync();

            return ApiResponse<List<ProductListDto>>.SuccessResult(items);
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto(c.CategoryId, c.CategoryName, c.Description ,c.Products.Count(), c.IsActive))
                .ToListAsync();

            return ApiResponse<List<CategoryDto>>.SuccessResult(categories);
        }
    }
}
