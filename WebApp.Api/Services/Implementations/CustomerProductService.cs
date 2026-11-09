using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Api.Data;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.FreeBook;
using WebApp.Shared.Dtos.CustomerDtos.Product;
using WebApp.Shared.Dtos.CustomerDtos.Review;
using WebApp.Shared.Enums;
using AuthorDto = WebApp.Shared.Dtos.Management.ContentAndMarketing.AuthorDto;
using CategoryDto = WebApp.Shared.Dtos.Management.ContentAndMarketing.CategoryDto;
using CustomerProductFilterDto = WebApp.Shared.Dtos.CustomerDtos.Product.ProductFilterDto;

namespace WebApp.Api.Services.Implementations
{
    public class CustomerProductService : ICustomerProductService
    {
        private readonly AppDbContext _context;

        public CustomerProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryId)
                .Select(c => new CategoryDto(
                    c.CategoryId,
                    c.CategoryName,
                    c.Description,
                    c.Products.Count(p => p.IsActive),
                    c.IsActive
                ))
                .ToListAsync();

            if (!categories.Any())
            {
                categories = GetFallbackCategories();
            }

            return ApiResponse<List<CategoryDto>>.SuccessResult(categories);
        }

        public async Task<ApiResponse<PagedResult<ProductListItemDto>>> GetProductsAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(p => EF.Functions.Like(p.Title, $"%{kw}%") ||
                                         (p.Author != null && EF.Functions.Like(p.Author.AuthorName, $"%{kw}%")));
            }

            if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => (p.Price * (1 - p.DiscountPercent / 100m)) >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => (p.Price * (1 - p.DiscountPercent / 100m)) <= filter.MaxPrice.Value);
            }

            query = filter.SortBy?.ToLowerInvariant() switch
            {
                "price-asc" => query.OrderBy(p => p.Price * (1 - p.DiscountPercent / 100m)),
                "price-desc" => query.OrderByDescending(p => p.Price * (1 - p.DiscountPercent / 100m)),
                "rating" => query.OrderByDescending(p => p.ProductReviews.Any() ? p.ProductReviews.Average(r => (double)r.Rating) : 5.0),
                "discount" => query.OrderByDescending(p => p.DiscountPercent),
                _ => query.OrderByDescending(p => p.ProductId)
            };

            var totalCount = await query.CountAsync();
            var page = Math.Max(1, filter.Page);
            var pageSize = filter.PageSize > 0 ? filter.PageSize : 12;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListItemDto(
                    p.ProductId,
                    p.Title,
                    p.Author.AuthorName,
                    p.Category.CategoryName,
                    p.Price,
                    p.DiscountPercent,
                    p.MainImageUrl,
                    p.StockQuantity,
                    p.IsActive,
                    p.ProductReviews.Any() ? p.ProductReviews.Average(r => (double)r.Rating) : 5.0
                ))
                .ToListAsync();

            if (totalCount == 0 && string.IsNullOrWhiteSpace(filter.Keyword))
            {
                return GetFallbackPagedProducts(filter);
            }

            var result = new PagedResult<ProductListItemDto>(items, totalCount, page, pageSize);
            return ApiResponse<PagedResult<ProductListItemDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<List<ProductListItemDto>>> GetFeaturedProductsAsync(int count = 8)
        {
            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.DiscountPercent)
                .ThenByDescending(p => p.ProductId)
                .Take(count)
                .Select(p => new ProductListItemDto(
                    p.ProductId,
                    p.Title,
                    p.Author.AuthorName,
                    p.Category.CategoryName,
                    p.Price,
                    p.DiscountPercent,
                    p.MainImageUrl,
                    p.StockQuantity,
                    p.IsActive,
                    p.ProductReviews.Any() ? p.ProductReviews.Average(r => (double)r.Rating) : 5.0
                ))
                .ToListAsync();

            if (!products.Any())
            {
                products = GetFallbackAllProducts().Take(count).ToList();
            }

            return ApiResponse<List<ProductListItemDto>>.SuccessResult(products);
        }

        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.Publisher)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductReviews)
                .FirstOrDefaultAsync(p => p.ProductId == id && p.IsActive);

            if (product == null)
            {
                var fallback = GetFallbackAllProducts().FirstOrDefault(p => p.ProductId == id);
                if (fallback != null)
                {
                    var fallbackDto = new ProductDto(
                        fallback.ProductId,
                        fallback.Title,
                        "Tác phẩm văn học kinh điển lưu truyền qua nhiều thế hệ độc giả Việt Nam.",
                        fallback.Price,
                        fallback.DiscountPercent,
                        fallback.StockQuantity,
                        fallback.MainImageUrl,
                        new List<string>(),
                        fallback.IsActive,
                        1,
                        fallback.CategoryName ?? "Văn Học",
                        1,
                        fallback.AuthorName ?? "Nhiều tác giả",
                        1,
                        "NXB Văn Học",
                        "Công ty Cổ phần Sách Văn Hóa",
                        fallback.AverageRating,
                        12
                    );
                    return ApiResponse<ProductDto>.SuccessResult(fallbackDto);
                }

                return ApiResponse<ProductDto>.FailureResult("Không tìm thấy sản phẩm.");
            }

            var dto = new ProductDto(
                product.ProductId,
                product.Title,
                product.Description,
                product.Price,
                product.DiscountPercent,
                product.StockQuantity,
                product.MainImageUrl,
                product.ProductImages.Select(img => img.ImageUrl).ToList(),
                product.IsActive,
                product.CategoryId,
                product.Category.CategoryName,
                product.AuthorId,
                product.Author.AuthorName,
                product.PublisherId,
                product.Publisher.PublisherName,
                product.SupplierName ?? string.Empty,
                product.ProductReviews.Any() ? product.ProductReviews.Average(r => (double)r.Rating) : 5.0,
                product.ProductReviews.Count
            );

            return ApiResponse<ProductDto>.SuccessResult(dto);
        }

        public async Task<ApiResponse<HomePageDataDto>> GetHomePageDataAsync()
        {
            // 1. Lấy Banners Active
            var banners = await _context.Banners
                .AsNoTracking()
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .Select(b => new BannerDto(
                    b.BannerId,
                    b.Title,
                    b.ImageUrl,
                    b.TargetUrl,
                    b.DisplayOrder
                ))
                .ToListAsync();

            if (!banners.Any())
            {
                banners = GetFallbackBanners();
            }

            // 2. Lấy Danh mục Active
            var catResponse = await GetCategoriesAsync();
            var categories = catResponse.Data ?? GetFallbackCategories();

            // 3. Sách Mới Nhất (8 sách)
            var newProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.ProductId)
                .Take(8)
                .Select(p => new ProductListItemDto(
                    p.ProductId,
                    p.Title,
                    p.Author.AuthorName,
                    p.Category.CategoryName,
                    p.Price,
                    p.DiscountPercent,
                    p.MainImageUrl,
                    p.StockQuantity,
                    p.IsActive,
                    p.ProductReviews.Any() ? p.ProductReviews.Average(r => (double)r.Rating) : 5.0
                ))
                .ToListAsync();

            if (!newProducts.Any())
            {
                newProducts = GetFallbackAllProducts().Take(8).ToList();
            }

            // 4. Sách Bán Chạy (Dựa trên số lượng mua trong OrderItems hoặc DiscountPercent cao)
            var bestSellerProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.OrderItems.Sum(oi => (int?)oi.Quantity) ?? 0)
                .ThenByDescending(p => p.DiscountPercent)
                .Take(8)
                .Select(p => new ProductListItemDto(
                    p.ProductId,
                    p.Title,
                    p.Author.AuthorName,
                    p.Category.CategoryName,
                    p.Price,
                    p.DiscountPercent,
                    p.MainImageUrl,
                    p.StockQuantity,
                    p.IsActive,
                    p.ProductReviews.Any() ? p.ProductReviews.Average(r => (double)r.Rating) : 5.0
                ))
                .ToListAsync();

            if (!bestSellerProducts.Any())
            {
                bestSellerProducts = GetFallbackAllProducts().OrderByDescending(p => p.DiscountPercent).Take(8).ToList();
            }

            // 5. Tủ sách 0 đồng (FreeBooks)
            var freeBooks = await _context.FreeBooks
                .AsNoTracking()
                .Where(fb => fb.IsActive && fb.AvailableQty > 0)
                .OrderByDescending(fb => fb.FreeBookId)
                .Take(6)
                .Select(fb => new FreeBookDto(
                    fb.FreeBookId,
                    fb.Title,
                    fb.AuthorName,
                    fb.Description,
                    fb.CoverImageUrl,
                    fb.AvailableQty,
                    fb.Condition,
                    fb.Source,
                    fb.IsActive
                ))
                .ToListAsync();

            if (!freeBooks.Any())
            {
                freeBooks = GetFallbackFreeBooks();
            }

            var homePageData = new HomePageDataDto(
                Banners: banners,
                Categories: categories,
                NewProducts: newProducts,
                BestSellerProducts: bestSellerProducts,
                FreeBooks: freeBooks
            );

            return ApiResponse<HomePageDataDto>.SuccessResult(homePageData);
        }

        public async Task<ApiResponse<List<AuthorDto>>> GetAuthorsAsync()
        {
            var authors = await _context.Authors
                .AsNoTracking()
                .Where(a => a.IsActive && a.Products.Any(p => p.IsActive))
                .OrderBy(a => a.AuthorName)
                .Select(a => new AuthorDto(
                    a.AuthorId,
                    a.AuthorName,
                    a.Biography,
                    null,
                    a.Products.Count(p => p.IsActive)
                ))
                .ToListAsync();

            if (!authors.Any())
            {
                authors = GetFallbackAuthors();
            }

            return ApiResponse<List<AuthorDto>>.SuccessResult(authors);
        }

        public async Task<ApiResponse<PagedResult<ProductReviewDto>>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 10, string? currentUserId = null)
        {
            var query = _context.ProductReviews
                .AsNoTracking()
                .Include(r => r.Customer)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();
            page = Math.Max(1, page);
            pageSize = pageSize > 0 ? pageSize : 10;

            var reviews = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Lấy danh sách ID của những user đã mua sản phẩm này trong đơn hàng Completed
            var customerIds = reviews.Select(r => r.CustomerId).Distinct().ToList();
            var verifiedCustomerIds = await _context.OrderItems
                .AsNoTracking()
                .Where(oi => oi.ProductId == productId &&
                             oi.Order.Status == (int)OrderStatus.Completed &&
                             customerIds.Contains(oi.Order.UserId))
                .Select(oi => oi.Order.UserId)
                .Distinct()
                .ToListAsync();

            var verifiedSet = new HashSet<string>(verifiedCustomerIds);

            var items = reviews.Select(r => new ProductReviewDto(
                ReviewId: r.ReviewId,
                UserId: r.CustomerId,
                ReviewerName: MaskCustomerName(r.Customer.FullName ?? r.Customer.UserName ?? "Khách hàng"),
                ReviewerAvatarUrl: r.Customer.AvatarUrl,
                Rating: r.Rating,
                Comment: r.Comment,
                CreatedAt: r.CreatedAt,
                IsVerifiedPurchase: verifiedSet.Contains(r.CustomerId),
                TotalLikes: 0,
                CurrentUserLiked: false
            )).ToList();

            var result = new PagedResult<ProductReviewDto>(items, totalCount, page, pageSize);
            return ApiResponse<PagedResult<ProductReviewDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<ProductReviewDto>> CreateProductReviewAsync(string userId, CreateReviewDto dto)
        {
            // 1. Kiểm tra sản phẩm có tồn tại không
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId && p.IsActive);
            if (product == null)
            {
                return ApiResponse<ProductReviewDto>.FailureResult("Sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
            }

            // 2. Kiểm tra tính xác thực của việc mua hàng:
            // Khách hàng phải có đơn hàng (OrderId nếu có cung cấp, hoặc bất kỳ đơn nào) ở trạng thái Completed chứa sản phẩm này
            var orderItemQuery = _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.ProductId == dto.ProductId &&
                             oi.Order.UserId == userId &&
                             oi.Order.Status == (int)OrderStatus.Completed);

            if (dto.OrderId > 0)
            {
                orderItemQuery = orderItemQuery.Where(oi => oi.OrderId == dto.OrderId);
            }

            var hasPurchased = await orderItemQuery.AnyAsync();
            if (!hasPurchased)
            {
                return ApiResponse<ProductReviewDto>.FailureResult("Bạn chỉ có thể đánh giá sản phẩm sau khi đã mua và nhận hàng thành công (đơn hàng Completed).");
            }

            // 3. Kiểm tra xem người dùng đã từng đánh giá sản phẩm này chưa
            var alreadyReviewed = await _context.ProductReviews
                .AnyAsync(r => r.ProductId == dto.ProductId && r.CustomerId == userId);

            if (alreadyReviewed)
            {
                return ApiResponse<ProductReviewDto>.FailureResult("Bạn đã đánh giá sản phẩm này rồi. Mỗi khách hàng chỉ được đánh giá một lần cho mỗi sản phẩm.");
            }

            // 4. Lưu đánh giá mới
            var review = new ProductReview
            {
                ProductId = dto.ProductId,
                CustomerId = userId,
                Rating = Math.Clamp(dto.Rating, 1, 5),
                Comment = dto.Comment?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

            var reviewDto = new ProductReviewDto(
                ReviewId: review.ReviewId,
                UserId: userId,
                ReviewerName: MaskCustomerName(user?.FullName ?? user?.UserName ?? "Khách hàng"),
                ReviewerAvatarUrl: user?.AvatarUrl,
                Rating: review.Rating,
                Comment: review.Comment,
                CreatedAt: review.CreatedAt,
                IsVerifiedPurchase: true,
                TotalLikes: 0,
                CurrentUserLiked: false
            );

            return ApiResponse<ProductReviewDto>.SuccessResult(reviewDto, "Gửi nhận xét và đánh giá thành công!");
        }

        private static string MaskCustomerName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "Khách hàng";
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1) return fullName;

            // Ví dụ: "Nguyễn Văn A" -> "Nguyễn V. A."
            if (parts.Length == 2)
            {
                return $"{parts[0]} {parts[1][0]}.";
            }

            return $"{parts[0]} {string.Join(" ", parts.Skip(1).Select(p => p[0] + "."))}";
        }

        private static List<BannerDto> GetFallbackBanners() => new()
        {
            new BannerDto(1, "Đại Tiệc Sách Văn Hóa 2026", "/images/banners/banner-1.jpg", "/books", 1),
            new BannerDto(2, "Tủ Sách Không Đồng - Lan Tỏa Tri Thức", "/images/banners/banner-2.jpg", "/free-books", 2),
            new BannerDto(3, "Tinh Hoa Văn Học Cổ Điển", "/images/banners/banner-3.jpg", "/categories/1", 3)
        };

        private static List<FreeBookDto> GetFallbackFreeBooks() => new()
        {
            new FreeBookDto(1, "Hạt Giống Tâm Hồn", "First News", "Sách truyền cảm hứng sống đẹp", "/images/covers/free-book-1.jpg", 10, "90%", "Cộng đồng quyên góp", true),
            new FreeBookDto(2, "Búp Sen Xanh", "Sơn Tùng", "Tiểu thuyết về thời niên thiếu của Bác Hồ", "/images/covers/free-book-2.jpg", 5, "85%", "Nhà xuất bản trao tặng", true),
            new FreeBookDto(3, "Đắc Nhân Tâm", "Dale Carnegie", "Nghệ thuật giao tiếp và đối nhân xử thế", "/images/covers/free-book-3.jpg", 8, "95%", "Bạn đọc thân thiết", true)
        };

        private static List<AuthorDto> GetFallbackAuthors() => new()
        {
            new AuthorDto(1, "Nguyễn Du", "Đại thi hào dân tộc Việt Nam, Danh nhân văn hóa thế giới", "/images/authors/nguyen-du.jpg", 5),
            new AuthorDto(2, "Ngô Sĩ Liên", "Nhà sử học lỗi lạc thời Lê sơ, tác giả Đại Việt Sử Ký Toàn Thư", "/images/authors/ngo-si-lien.jpg", 3),
            new AuthorDto(3, "Vũ Trọng Phụng", "Cây bút trào phúng bậc thầy của văn học hiện thực Việt Nam", "/images/authors/vu-trong-phung.jpg", 4),
            new AuthorDto(4, "Nam Cao", "Nhà văn hiện thực xuất sắc với ngòi bút sắc lạnh giàu nhân đạo", "/images/authors/nam-cao.jpg", 6)
        };

        private static List<CategoryDto> GetFallbackCategories() => new()
        {
            new CategoryDto(1, "Văn Học Cổ Điển", "Các danh tác văn chương trường tồn theo năm tháng", 2, true),
            new CategoryDto(2, "Sử Việt & Dã Sử", "Dấu ấn thăng trầm qua các triều đại lịch sử", 3, true),
            new CategoryDto(3, "Văn Học Hiện Thực", "Giai đoạn văn học 1930 - 1945", 3, true),
            new CategoryDto(4, "Triết Học Đông Phương", "Minh triết Á Đông, Nho - Y - Lý - Số", 2, true),
            new CategoryDto(5, "Thơ Ca & Tản Văn", "Những vần thơ trác tuyệt của dân tộc", 2, true)
        };

        private static List<ProductListItemDto> GetFallbackAllProducts() => new()
        {
            new ProductListItemDto(1, "Truyện Kiều (Bản Duy Minh Thị 1872)", "Nguyễn Du", "Văn Học Cổ Điển", 185000, 15, "/images/covers/truyen-kieu.svg", 50, true, 4.9),
            new ProductListItemDto(2, "Đại Việt Sử Ký Toàn Thư (Trọn bộ)", "Ngô Sĩ Liên", "Sử Việt & Dã Sử", 350000, 0, "/images/covers/dai-viet-su-ky.svg", 20, true, 5.0),
            new ProductListItemDto(3, "Việt Nam Sử Lược", "Trần Trọng Kim", "Sử Việt & Dã Sử", 220000, 10, "/images/covers/viet-nam-su-luoc.svg", 35, true, 4.8),
            new ProductListItemDto(4, "Số Đỏ (Bản bìa cứng)", "Vũ Trọng Phụng", "Văn Học Hiện Thực", 160000, 20, "/images/covers/so-do.svg", 40, true, 4.7),
            new ProductListItemDto(5, "Tắt Đèn", "Ngô Tất Tố", "Văn Học Hiện Thực", 125000, 10, "/images/covers/tat-den.svg", 60, true, 4.6),
            new ProductListItemDto(6, "Đạo Đức Kinh (Nguyên tác và Bình giải)", "Lão Tử", "Triết Học Đông Phương", 195000, 5, "/images/covers/dao-duc-kinh.svg", 25, true, 4.9),
            new ProductListItemDto(7, "Chinh Phụ Ngâm (Bản diễn Nôm)", "Đoàn Thị Điểm", "Thơ Ca & Tản Văn", 140000, 10, "/images/covers/chinh-phu-ngam.svg", 30, true, 4.8),
            new ProductListItemDto(8, "Nam Quốc Sơn Hà (Khảo luận tư tưởng)", "Lý Thường Kiệt", "Thơ Ca & Tản Văn", 110000, 0, "/images/covers/nam-quoc-son-ha.svg", 45, true, 4.9),
            new ProductListItemDto(9, "Lão Hạc & Tuyển Tập Truyện Ngắn", "Nam Cao", "Văn Học Hiện Thực", 135000, 15, "", 28, true, 4.8),
            new ProductListItemDto(10, "Lục Vân Tiên (Bản truyền kỳ)", "Nguyễn Đình Chiểu", "Văn Học Cổ Điển", 155000, 10, "", 22, true, 4.7),
            new ProductListItemDto(11, "Hoàng Lê Nhất Thống Chí", "Ngô Gia Văn Phái", "Sử Việt & Dã Sử", 260000, 15, "", 18, true, 4.8),
            new ProductListItemDto(12, "Luận Ngữ (Khảo dịch tinh hoa)", "Khổng Tử", "Triết Học Đông Phương", 210000, 10, "", 32, true, 4.8)
        };

        private static ApiResponse<PagedResult<ProductListItemDto>> GetFallbackPagedProducts(ProductFilterDto filter)
        {
            var query = GetFallbackAllProducts().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(p => p.Title.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         (p.AuthorName != null && p.AuthorName.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
            {
                var catName = GetFallbackCategories().FirstOrDefault(c => c.CategoryId == filter.CategoryId.Value)?.CategoryName;
                if (!string.IsNullOrEmpty(catName))
                {
                    query = query.Where(p => string.Equals(p.CategoryName, catName, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.FinalPrice >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.FinalPrice <= filter.MaxPrice.Value);
            }

            query = filter.SortBy?.ToLowerInvariant() switch
            {
                "price-asc" => query.OrderBy(p => p.FinalPrice),
                "price-desc" => query.OrderByDescending(p => p.FinalPrice),
                "rating" => query.OrderByDescending(p => p.AverageRating),
                "discount" => query.OrderByDescending(p => p.DiscountPercent),
                _ => query.OrderBy(p => p.ProductId)
            };

            var totalCount = query.Count();
            var page = Math.Max(1, filter.Page);
            var pageSize = filter.PageSize > 0 ? filter.PageSize : 12;
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = new PagedResult<ProductListItemDto>(items, totalCount, page, pageSize);
            return ApiResponse<PagedResult<ProductListItemDto>>.SuccessResult(result);
        }
    }
}
