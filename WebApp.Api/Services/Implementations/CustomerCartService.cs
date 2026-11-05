using Microsoft.EntityFrameworkCore;
using WebApp.Api.Data;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Cart;

namespace WebApp.Api.Services.Implementations
{
    public class CustomerCartService : ICustomerCartService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        // In-memory fallback cart cho guest/khi DB trống hoặc chưa có tài khoản
        private static readonly Dictionary<string, List<CartItemDto>> _guestCarts = new();

        public CustomerCartService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<CartDto>> GetCartAsync()
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                var guestItems = GetOrCreateGuestCart("guest");
                return ApiResponse<CartDto>.SuccessResult(new CartDto(guestItems));
            }

            var cart = await _context.Carts
                .AsNoTracking()
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return ApiResponse<CartDto>.SuccessResult(new CartDto(new List<CartItemDto>()));
            }

            var items = cart.CartItems
                .Where(ci => ci.Product.IsActive)
                .Select(ci => new CartItemDto(
                    ci.ProductId,
                    ci.Product.Title,
                    ci.Product.MainImageUrl,
                    ci.Product.Price,
                    ci.Product.Price * (1 - ci.Product.DiscountPercent / 100m),
                    ci.Quantity,
                    ci.Product.StockQuantity
                ))
                .ToList();

            return ApiResponse<CartDto>.SuccessResult(new CartDto(items));
        }

        public async Task<ApiResponse<CartDto>> AddToCartAsync(AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
            {
                return ApiResponse<CartDto>.FailureResult("Số lượng sản phẩm không hợp lệ.");
            }

            var userId = _currentUserService.UserId;

            // Nếu người dùng chưa đăng nhập, thao tác trên In-memory Cart
            if (string.IsNullOrEmpty(userId))
            {
                var guestItems = GetOrCreateGuestCart("guest");
                var existing = guestItems.FirstOrDefault(i => i.ProductId == dto.ProductId);

                if (existing != null)
                {
                    var updated = existing with { Quantity = existing.Quantity + dto.Quantity };
                    guestItems.Remove(existing);
                    guestItems.Add(updated);
                }
                else
                {
                    // Lấy thông tin sản phẩm
                    var p = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == dto.ProductId);
                    if (p != null)
                    {
                        guestItems.Add(new CartItemDto(
                            p.ProductId,
                            p.Title,
                            p.MainImageUrl,
                            p.Price,
                            p.Price * (1 - p.DiscountPercent / 100m),
                            dto.Quantity,
                            p.StockQuantity
                        ));
                    }
                    else
                    {
                        // Fallback sản phẩm mẫu nếu DB chưa seed
                        guestItems.Add(new CartItemDto(
                            dto.ProductId,
                            $"Tác phẩm cổ điển #{dto.ProductId}",
                            "/images/book-placeholder.svg",
                            150000,
                            150000,
                            dto.Quantity,
                            50
                        ));
                    }
                }

                return ApiResponse<CartDto>.SuccessResult(new CartDto(guestItems));
            }

            // Người dùng đã đăng nhập: xử lý DB với transaction/lock tối ưu
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId && p.IsActive);
            if (product == null)
            {
                return ApiResponse<CartDto>.FailureResult("Sách không tồn tại hoặc đã ngừng kinh doanh.");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (cartItem != null)
            {
                cartItem.Quantity += dto.Quantity;
                if (cartItem.Quantity > product.StockQuantity)
                {
                    cartItem.Quantity = product.StockQuantity;
                }
            }
            else
            {
                var qty = Math.Min(dto.Quantity, product.StockQuantity);
                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = dto.ProductId,
                    Quantity = qty
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetCartAsync();
        }

        public async Task<ApiResponse<CartDto>> UpdateCartItemAsync(UpdateCartItemDto dto)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                var guestItems = GetOrCreateGuestCart("guest");
                var existing = guestItems.FirstOrDefault(i => i.ProductId == dto.ProductId);
                if (existing != null)
                {
                    guestItems.Remove(existing);
                    if (dto.Quantity > 0)
                    {
                        guestItems.Add(existing with { Quantity = dto.Quantity });
                    }
                }
                return ApiResponse<CartDto>.SuccessResult(new CartDto(guestItems));
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return ApiResponse<CartDto>.FailureResult("Giỏ hàng không tồn tại.");
            }

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (item != null)
            {
                if (dto.Quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = dto.Quantity;
                }
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return await GetCartAsync();
        }

        public async Task<ApiResponse<CartDto>> RemoveCartItemAsync(int productId)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                var guestItems = GetOrCreateGuestCart("guest");
                guestItems.RemoveAll(i => i.ProductId == productId);
                return ApiResponse<CartDto>.SuccessResult(new CartDto(guestItems));
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (item != null)
                {
                    _context.CartItems.Remove(item);
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            return await GetCartAsync();
        }

        public async Task<ApiResponse<bool>> ClearCartAsync()
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                var guestItems = GetOrCreateGuestCart("guest");
                guestItems.Clear();
                return ApiResponse<bool>.SuccessResult(true, "Đã làm trống giỏ hàng.");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return ApiResponse<bool>.SuccessResult(true, "Đã làm trống giỏ hàng.");
        }

        private static List<CartItemDto> GetOrCreateGuestCart(string key)
        {
            lock (_guestCarts)
            {
                if (!_guestCarts.TryGetValue(key, out var list))
                {
                    list = new List<CartItemDto>();
                    _guestCarts[key] = list;
                }
                return list;
            }
        }
    }
}
