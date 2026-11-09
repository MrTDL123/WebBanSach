using Microsoft.EntityFrameworkCore;
using WebApp.Api.Data;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Api.Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public ContentService(AppDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm với phân trang và lọc.
        /// </summary> 
        public async Task<ApiResponse<PagedResultDto<ProductListDto>>> GetProductsAsync(ProductFilterDto filterDto)
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

            var totalItems = await query.CountAsync();

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

            var pagedResult = new PagedResultDto<ProductListDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = filterDto.PageIndex,
                PageSize = filterDto.PageSize
            };
            return ApiResponse<PagedResultDto<ProductListDto>>.SuccessResult(pagedResult);
        }

        /// <summary>
        /// Quản lý sản phẩm: Lấy thông tin chi tiết, xóa, tạo mới và cập nhật sản phẩm.
        /// </summary>
        public async Task<ApiResponse<bool>> UpdateProductAsync(UpdateProductDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

                if (product == null) return ApiResponse<bool>.FailureResult("Sản phẩm không tồn tại.");

                product.Title = dto.Title ?? "";
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.DiscountPercent = dto.DiscountPercent;
                product.SupplierName = dto.SupplierName;
                product.CategoryId = dto.CategoryId;
                product.AuthorId = dto.AuthorId;
                product.PublisherId = dto.PublisherId;
                product.IsActive = dto.IsActive;
                product.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(dto.MainImageBase64))
                {
                    _fileStorageService.DeletePhysicalFile(product.MainImageUrl);
                    product.MainImageUrl = await _fileStorageService.SaveBase64ImageAsync(
                        dto.MainImageBase64, 
                        $"uploads/products/{product.ProductId}", 
                        "main");
                }

                // 3. Cập nhật Album ảnh phụ
                if (dto.GalleryImages != null)
                {
                    foreach (var item in dto.GalleryImages)
                    {
                        // TH1: Ảnh phụ bị đánh dấu Xóa
                        if (item.IsDeleted && item.ImageId > 0)
                        {
                            var existingImg = product.ProductImages.FirstOrDefault(x => x.ImageId == item.ImageId);
                            if (existingImg != null)
                            {
                                _fileStorageService.DeletePhysicalFile(existingImg.ImageUrl); // Xóa file vật lý
                                _context.ProductImages.Remove(existingImg); // Xóa trong DB
                            }
                        }
                        // TH2: Thêm mới ảnh phụ
                        else if (item.ImageId == 0 && !string.IsNullOrEmpty(item.Base64Data))
                        {
                            _context.ProductImages.Add(new ProductImage
                            {
                                ProductId = product.ProductId,
                                ImageUrl = await _fileStorageService.SaveBase64ImageAsync(
                                    item.Base64Data, 
                                    $"uploads/products/{product.ProductId}", 
                                    $"gallery_{item.DisplayOrder}"),
                                DisplayOrder = item.DisplayOrder
                            });
                        }
                        // TH3: Cập nhật lại Thứ tự hiển thị (DisplayOrder) của ảnh cũ
                        else if (item.ImageId > 0 && !item.IsDeleted)
                        {
                            var existingImg = product.ProductImages.FirstOrDefault(x => x.ImageId == item.ImageId);
                            if (existingImg != null)
                            {
                                existingImg.DisplayOrder = item.DisplayOrder;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<bool>.SuccessResult(true, "Cập nhật sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<bool>.FailureResult($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CreateProductAsync(CreateProductDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = new Product
                {
                    Title = dto.Title ?? "",
                    Description = dto.Description,
                    Price = dto.Price,
                    DiscountPercent = dto.DiscountPercent,
                    SupplierName = dto.SupplierName,
                    CategoryId = dto.CategoryId,
                    AuthorId = dto.AuthorId,
                    PublisherId = dto.PublisherId,
                    MainImageUrl = string.Empty,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.AddAsync(product);
                await _context.SaveChangesAsync(); // Lưu để SQL tự sinh ra product.ProductId

                if(!string.IsNullOrEmpty(dto.MainImageBase64))
                {

                    product.MainImageUrl = await _fileStorageService.SaveBase64ImageAsync(
                        dto.MainImageBase64, 
                        $"uploads/products/{product.ProductId}", 
                        "main");
                }

                // 3. Lưu Danh sách Ảnh phụ (Gallery Images)
                if (dto.GalleryImages != null && dto.GalleryImages.Any())
                {
                    foreach (var item in dto.GalleryImages)
                    {
                        if (!string.IsNullOrEmpty(item.Base64Data))
                        {
                            var productImage = new ProductImage
                            {
                                ProductId = product.ProductId,
                                ImageUrl = await _fileStorageService.SaveBase64ImageAsync(
                                    item.Base64Data, 
                                    $"uploads/products/{product.ProductId}", 
                                    $"gallery_{item.DisplayOrder}"),
                                DisplayOrder = item.DisplayOrder
                            };
                            _context.ProductImages.Add(productImage);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<bool>.SuccessResult(true, "Thêm sản phẩm mới thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<bool>.FailureResult($"Lỗi khi tạo sản phẩm: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UpdateProductDto>> GetProductForUpdateAsync(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(p => new UpdateProductDto
                {
                    ProductId = p.ProductId,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountPercent = p.DiscountPercent,
                    StockQuantity = p.StockQuantity,
                    SupplierName = p.SupplierName,
                    CategoryId = p.CategoryId,
                    AuthorId = p.AuthorId,
                    PublisherId = p.PublisherId,
                    IsActive = p.IsActive,
                    MainImageUrl = p.MainImageUrl,
                    GalleryImages = p.ProductImages
                        .OrderBy(img => img.DisplayOrder)
                        .Select(img => new ProductGalleryItemDto
                        {
                            ImageId = img.ImageId,
                            ImageUrl = img.ImageUrl,
                            DisplayOrder = img.DisplayOrder,
                            IsDeleted = false
                        }).ToList() // Load Album ảnh phụ
                })
                .FirstOrDefaultAsync();
            if (product == null)
            {
                return ApiResponse<UpdateProductDto>.FailureResult("Sản phẩm không tồn tại");
            }
            return ApiResponse<UpdateProductDto>.SuccessResult(product);
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            var product = await _context.Products
                .Where(p => p.ProductId == id)
                .FirstOrDefaultAsync();

            if(product == null)
            {
                return ApiResponse<bool>.FailureResult("Sản phẩm không tồn tại");
            }

            bool hasOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
            bool hasPurchaseOrders = await _context.PurchaseOrderItems.AnyAsync(poi =>  poi.ProductId == id);

            if (hasOrders || hasPurchaseOrders)
            {
                return ApiResponse<bool>.FailureResult("Không thể xóa sản phẩm này vì đã có lịch sử đơn hàng hoặc phiếu nhập kho liên quan. Vui lòng chuyển sang trạng thái Ngừng kinh doanh.");
            }

            try
            {
                _fileStorageService.DeletePhysicalFolder($"uploads/products/{product.ProductId}");
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi xóa sản phẩm: {ex.Message}");
            }

            return ApiResponse<bool>.SuccessResult(true, "Xóa sản phẩm thành công");
        }

        /// <summary>
        /// Lấy danh sách sách tủ sách 0 đồng với phân trang và lọc.
        /// </summary>
        public async Task<ApiResponse<PagedResultDto<FreeBookListDto>>> GetFreeBooksAsync(FreeBookFilterDto filterDto)
        {
            var query = _context.FreeBooks.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
            {
                var term = filterDto.SearchTerm.Trim().ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(term)
                                        || b.AuthorName.ToLower().Contains(term));
            }

            // Vì trong ProductFilterDto CategoryId được khai báo kiểu dữ liệu bool? (Nullable bool) có thuộc
            //  tính .HashValue để kiểm tra xem biến này có chứa giá trị (true hoặc false) hay đang bị null.
            // Tương tự như ở trên categoryid
            if (filterDto.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == filterDto.IsActive.Value);
            }

            // 3. Lọc tồn kho
            if (filterDto.StockStatus == "available")
            {
                query = query.Where(b => b.AvailableQty > 0);
            }
            else if (filterDto.StockStatus == "out_of_stock")
            {
                query = query.Where(b => b.AvailableQty == 0);
            }
            // 4. Sắp xếp
            query = filterDto.SortBy switch
            {
                "most_requested" => query.OrderByDescending(b => b.TotalRequestedQty),
                "most_donated" => query.OrderByDescending(b => b.TotalDonatedQty),
                _ => query.OrderByDescending(b => b.CreatedAt) // Mặc định: Mới nhất
            };

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filterDto.PageIndex - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .Select(b => new FreeBookListDto(
                    b.FreeBookId,
                    b.Title,
                    b.AuthorName,
                    b.CoverImageUrl,
                    b.AvailableQty,
                    b.TotalRequestedQty,
                    b.TotalDonatedQty,
                    b.Condition,
                    b.Source,
                    b.IsActive,
                    b.CreatedAt
                )).ToListAsync();

            var pagedResult = new PagedResultDto<FreeBookListDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = filterDto.PageIndex,
                PageSize = filterDto.PageSize
            };
            return ApiResponse<PagedResultDto<FreeBookListDto>>.SuccessResult(pagedResult);
        }

        /// <summary>
        /// Quản lý sách tủ sách 0 đồng: Lấy thông tin chi tiết, xóa, tạo mới và cập nhật sách tủ sách 0 đồng.
        /// </summary>
        public async Task<ApiResponse<bool>> UpdateFreeBookAsync(UpdateFreeBookDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var freeBook = await _context.FreeBooks
                    .FirstOrDefaultAsync(fb => fb.FreeBookId == dto.FreeBookId);

                if (freeBook == null) return ApiResponse<bool>.FailureResult("sách 0 đồng không tồn tại.");

                freeBook.Title = dto.Title;
                freeBook.AuthorName = dto.AuthorName;
                freeBook.Description = dto.Description;
                freeBook.Condition = dto.Condition;
                freeBook.Source = dto.Source;
                freeBook.IsActive = dto.IsActive;
                freeBook.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(dto.CoverImageBase64))
                {
                    _fileStorageService.DeletePhysicalFile(freeBook.CoverImageUrl);
                    freeBook.CoverImageUrl = await _fileStorageService.SaveBase64ImageAsync(
                        dto.CoverImageBase64,
                        $"uploads/freebooks/{freeBook.FreeBookId}",
                        "main");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<bool>.SuccessResult(true, "Cập nhật sách 0 đồng thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<bool>.FailureResult($"Lỗi khi cập nhật sách 0 đồng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CreateFreeBookAsync(CreateFreeBookDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var freeBook = new FreeBook
                {
                    Title = dto.Title,
                    AuthorName = dto.AuthorName,
                    Description = dto.Description,
                    CoverImageUrl = string.Empty,
                    AvailableQty = 0,
                    TotalDonatedQty = 0,
                    TotalRequestedQty = 0,
                    Condition = dto.Condition,
                    Source = dto.Source,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.AddAsync(freeBook);
                await _context.SaveChangesAsync(); // Lưu để SQL tự sinh ra product.ProductId

                if (!string.IsNullOrEmpty(dto.CoverImageBase64))
                {

                    freeBook.CoverImageUrl = await _fileStorageService.SaveBase64ImageAsync(
                        dto.CoverImageBase64,
                        $"uploads/freebooks/{freeBook.FreeBookId}",
                        "main");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<bool>.SuccessResult(true, "Thêm sách 0 đồng mới thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<bool>.FailureResult($"Lỗi khi tạo sách 0 đồng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UpdateFreeBookDto>> GetFreeBookForUpdateAsync(int id)
        {
            var freeBook = await _context.FreeBooks
                .AsNoTracking()
                .Where(fb => fb.FreeBookId == id)
                .Select(fb => new UpdateFreeBookDto
                {
                    FreeBookId = fb.FreeBookId,
                    Title = fb.Title,
                    AuthorName = fb.AuthorName,
                    Description = fb.Description,
                    AvailableQty = fb.AvailableQty,
                    Condition = fb.Condition,
                    Source = fb.Source,
                    IsActive = fb.IsActive,
                    CoverImageUrl = fb.CoverImageUrl
                })
                .FirstOrDefaultAsync();
            if (freeBook == null)
            {
                return ApiResponse<UpdateFreeBookDto>.FailureResult("sách 0 đồng không tồn tại");
            }
            return ApiResponse<UpdateFreeBookDto>.SuccessResult(freeBook);
        }

        public async Task<ApiResponse<FreeBookDetailDto>> GetFreeBookDetailAsync(int id)
        {
            var freeBook = await _context.FreeBooks
                .AsNoTracking()
                .Where(fb => fb.FreeBookId == id)
                .Select(fb => new FreeBookDetailDto
                {
                    FreeBookId = fb.FreeBookId,
                    Title = fb.Title,
                    AuthorName = fb.AuthorName,
                    Description = fb.Description,
                    CoverImageUrl = fb.CoverImageUrl,
                    AvailableQty = fb.AvailableQty,
                    TotalDonatedQty = fb.TotalDonatedQty,
                    TotalRequestedQty = fb.TotalRequestedQty,
                    Condition = fb.Condition,
                    Source = fb.Source,
                    IsActive = fb.IsActive,
                    CreatedAt = fb.CreatedAt,
                    UpdatedAt = fb.UpdatedAt,
                    DonationHistories = fb.BookDonationItems.Select(d => new FreeBookDonationHistoryDto(
                        d.DonationId,
                        d.Donation.DonationCode,
                        d.Donation.DonorName,
                        d.Donation.DonorPhone,
                        d.ActualQuantityReceived ?? d.Quantity,
                        d.Donation.DonatedAt
                    )).ToList(),
                    RequestHistories = fb.BookRequests.Select(r => new FreeBookRequestHistoryDto(
                        r.RequestId,
                        r.RequestCode,
                        r.ReceiverName,
                        r.ReceiverPhone,
                        r.QuantityRequested,
                        r.Reason,
                        r.Status,
                        r.RequestedAt
                    )).ToList()
                })
                .FirstOrDefaultAsync();

            if (freeBook == null)
            {
                return ApiResponse<FreeBookDetailDto>.FailureResult("Sách 0 đồng không tồn tại.");
            }
            return ApiResponse<FreeBookDetailDto>.SuccessResult(freeBook);
        }

        public async Task<ApiResponse<bool>> DeleteFreeBookAsync(int id)
        {
            var freeBook = await _context.FreeBooks
                .Where(p => p.FreeBookId == id)
                .FirstOrDefaultAsync();

            if (freeBook == null)
            {
                return ApiResponse<bool>.FailureResult("sách 0 đồng không tồn tại");
            }

            bool hasRequest = await _context.BookRequests.AnyAsync(r => r.FreeBookId == id);
            bool hasDonationItems = await _context.BookDonationItems.AnyAsync(d => d.LinkedFreeBookId == id);
            if (hasRequest || hasDonationItems)
            {
                return ApiResponse<bool>.FailureResult("Không thể xóa sách này vì đã có lịch sử ủng hộ hoặc yêu cầu thỉnh sách liên quan. Vui lòng chuyển sang trạng thái ẩn.");
            }

            try
            {
                _fileStorageService.DeletePhysicalFolder($"uploads/freebooks/{freeBook.FreeBookId}");
                _context.FreeBooks.Remove(freeBook);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Xóa sách 0 đồng thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi xóa sách 0 đồng: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .Select(c => new CategoryDto(
                    c.CategoryId, 
                    c.CategoryName, 
                    c.Description ,
                    c.Products.Count(), 
                    c.IsActive
                ))
                .ToListAsync();

            if (categories == null || !categories.Any())
            {
                return ApiResponse<List<CategoryDto>>.FailureResult("Không có danh mục nào.");
            }

            return ApiResponse<List<CategoryDto>>.SuccessResult(categories);
        }

        public async Task<ApiResponse<List<AuthorDto>>> GetAuthorsAsync()
        {
            var authors = await _context.Authors
                .AsNoTracking()
                .Where(a => a.IsActive)
                .Select(a => new AuthorDto(
                    a.AuthorId, 
                    a.AuthorName, 
                    a.Biography, 
                    a.Products.Count(), 
                    a.IsActive
                ))
                .ToListAsync();

            if (authors == null || !authors.Any())
            {
                return ApiResponse<List<AuthorDto>>.FailureResult("Không có tác giả nào.");
            }

            return ApiResponse<List<AuthorDto>>.SuccessResult(authors);
        }

        public async Task<ApiResponse<List<PublisherDto>>> GetPublishersAsync()
        {
            var publishers = await _context.Publishers
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Select(p => new PublisherDto(
                    p.PublisherId, 
                    p.PublisherName, 
                    p.Address, 
                    p.PhoneNumber, 
                    p.Products.Count(), 
                    p.IsActive
                ))
                .ToListAsync();

            if (publishers == null || !publishers.Any())
            {
                return ApiResponse<List<PublisherDto>>.FailureResult("Không có nhà xuất bản nào.");
            }

            return ApiResponse<List<PublisherDto>>.SuccessResult(publishers);
        }

        public async Task<ApiResponse<List<VoucherDto>>> GetVouchersAsync()
        {
            var vouchers = await _context.Coupons
                .AsNoTracking()
                .Where(v => v.IsActive)
                .Select(v => new VoucherDto(
                    v.CouponId, 
                    v.Code, 
                    null, 
                    v.DiscountAmount,
                    0,
                    v.MinOrderAmount, 
                    0,
                    v.UsageLimit,
                    v.UsageLimit - v.TimesUsed,
                    v.EndDate,
                    v.IsActive
                ))
                .ToListAsync();

            if (vouchers == null || !vouchers.Any())
            {
                return ApiResponse<List<VoucherDto>>.FailureResult("Không có voucher nào.");
            }

            return ApiResponse<List<VoucherDto>>.SuccessResult(vouchers);
        }

        public async Task<ApiResponse<bool>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName ?? "",
                Description = dto.Description,
                IsActive = true
            };

            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Tạo danh mục thành công.");
            }
            catch (Exception ex) 
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi tạo danh mục: {ex.Message}");
            }  
        }

        public async Task<ApiResponse<bool>> CreateAuthorAsync(CreateAuthorDto dto)
        {
            var author = new Author
            {
                AuthorName = dto.AuthorName ?? "",
                Biography = dto.Biography,
                IsActive = true
            };

            try
            {
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Tạo tác giả thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi tạo tác giả: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CreatePublisherAsync(CreatePublisherDto dto)
        {
            var publisher = new Publisher
            {
                PublisherName = dto.PublisherName ?? "",
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true
            };

            try
            {
                _context.Publishers.Add(publisher);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Tạo nhà xuất bản thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi tạo nhà xuất bản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CreateVoucherAsync(CreateVoucherDto dto)
        {
            var voucher = new Coupon
            {
                Code = dto.Code ?? "",
                DiscountAmount = dto.DiscountAmount,
                MinOrderAmount = dto.MinOrderAmount,
                UsageLimit = dto.TotalUsageLimit,
                EndDate = dto.ExpiredAt,
                IsActive = true
            };

            try
            {
                _context.Coupons.Add(voucher);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Tạo voucher thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi tạo voucher: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(dto.CategoryId);
                if (category == null) return ApiResponse<bool>.FailureResult("Danh mục không tồn tại.");

                category.CategoryName = dto.CategoryName ?? "";
                category.Description = dto.Description;
                category.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Cập nhật danh mục thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi cập nhật danh mục: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAuthorAsync(UpdateAuthorDto dto)
        {
            try
            {
                var author = await _context.Authors.FindAsync(dto.AuthorId);
                if (author == null) return ApiResponse<bool>.FailureResult("Tác giả không tồn tại.");

                author.AuthorName = dto.AuthorName ?? "";
                author.Biography = dto.Biography;
                author.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Cập nhật tác giả thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi cập nhật tác giả: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdatePublisherAsync(UpdatePublisherDto dto)
        {
            try
            {
                var publisher = await _context.Publishers.FindAsync(dto.PublisherId);
                if (publisher == null) return ApiResponse<bool>.FailureResult("Nhà xuất bản không tồn tại.");

                publisher.PublisherName = dto.PublisherName ?? "";
                publisher.Address = dto.Address;
                publisher.PhoneNumber = dto.PhoneNumber;
                publisher.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Cập nhật nhà xuất bản thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi cập nhật nhà xuất bản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateVoucherAsync(UpdateVoucherDto dto)
        {
            try
            {
                var voucher = await _context.Coupons.FindAsync(dto.VoucherId);
                if (voucher == null) return ApiResponse<bool>.FailureResult("Voucher không tồn tại.");

                voucher.Code = dto.Code ?? "";
                voucher.DiscountAmount = dto.DiscountAmount;
                voucher.MinOrderAmount = dto.MinOrderAmount;
                voucher.UsageLimit = dto.TotalUsageLimit;
                voucher.EndDate = dto.ExpiredAt;
                voucher.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Cập nhật voucher thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi cập nhật voucher: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null) return ApiResponse<bool>.FailureResult("Danh mục không tồn tại.");
                bool hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
                if (hasProducts)
                {
                    return ApiResponse<bool>.FailureResult("Không thể xóa danh mục này vì vẫn còn sản phẩm liên quan.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Xóa danh mục thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi xóa danh mục: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAuthorAsync(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null) return ApiResponse<bool>.FailureResult("Tác giả không tồn tại.");
                bool hasProducts = await _context.Products.AnyAsync(p => p.AuthorId == id);
                if (hasProducts)
                {
                    return ApiResponse<bool>.FailureResult("Không thể xóa tác giả này vì vẫn còn sản phẩm liên quan.");
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Xóa tác giả thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi xóa tác giả: {ex.Message}");
            } 
        }

        public async Task<ApiResponse<bool>> DeletePublisherAsync(int id)
        {
            try
            {
                var publisher = await _context.Publishers.FindAsync(id);
                if (publisher == null) return ApiResponse<bool>.FailureResult("Nhà xuất bản không tồn tại.");
                bool hasProducts = await _context.Products.AnyAsync(p => p.PublisherId == id);
                if (hasProducts)
                {
                    return ApiResponse<bool>.FailureResult("Không thể xóa nhà xuất bản này vì vẫn còn sản phẩm liên quan.");
                }

                _context.Publishers.Remove(publisher);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Xóa nhà xuất bản thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi xóa nhà xuất bản: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteVoucherAsync(int id)
        {
            try
            {
                var voucher = await _context.Coupons.FindAsync(id);
                if (voucher == null) return ApiResponse<bool>.FailureResult("Voucher không tồn tại.");

                _context.Coupons.Remove(voucher);
                await _context.SaveChangesAsync();
                return ApiResponse<bool>.SuccessResult(true, "Xóa voucher thành công.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResult($"Lỗi khi xóa voucher: {ex.Message}");
            }
        }
    }
}
