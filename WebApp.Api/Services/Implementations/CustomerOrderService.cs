using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Api.Data;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.CustomerDtos.Order;
using WebApp.Shared.Enums;

namespace WebApp.Api.Services.Implementations
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly AppDbContext _context;

        public CustomerOrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<VoucherApplyResultDto>> ApplyVoucherAsync(ApplyVoucherDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                return ApiResponse<VoucherApplyResultDto>.FailureResult("Mã giảm giá không được để trống.");
            }

            var cleanCode = dto.Code.Trim().ToUpperInvariant();
            var now = DateTime.UtcNow;

            var coupon = await _context.Coupons
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == cleanCode && c.IsActive);

            if (coupon == null)
            {
                var failure = new VoucherApplyResultDto(false, "Mã giảm giá không tồn tại hoặc đã hết hiệu lực.", 0, dto.CurrentOrderAmount);
                return ApiResponse<VoucherApplyResultDto>.SuccessResult(failure);
            }

            if (now < coupon.StartDate || now > coupon.EndDate)
            {
                var failure = new VoucherApplyResultDto(false, "Mã giảm giá chưa đến ngày áp dụng hoặc đã hết hạn.", 0, dto.CurrentOrderAmount);
                return ApiResponse<VoucherApplyResultDto>.SuccessResult(failure);
            }

            if (coupon.UsageLimit > 0 && coupon.TimesUsed >= coupon.UsageLimit)
            {
                var failure = new VoucherApplyResultDto(false, "Mã giảm giá đã đạt số lượt sử dụng tối đa.", 0, dto.CurrentOrderAmount);
                return ApiResponse<VoucherApplyResultDto>.SuccessResult(failure);
            }

            if (dto.CurrentOrderAmount < coupon.MinOrderAmount)
            {
                var failure = new VoucherApplyResultDto(
                    false,
                    $"Đơn hàng tối thiểu phải từ {coupon.MinOrderAmount:N0}đ để sử dụng mã này.",
                    0,
                    dto.CurrentOrderAmount
                );
                return ApiResponse<VoucherApplyResultDto>.SuccessResult(failure);
            }

            var discount = Math.Min(coupon.DiscountAmount, dto.CurrentOrderAmount);
            var finalAmount = Math.Max(0, dto.CurrentOrderAmount - discount);

            var result = new VoucherApplyResultDto(true, null, discount, finalAmount);
            return ApiResponse<VoucherApplyResultDto>.SuccessResult(result, "Áp dụng mã giảm giá thành công.");
        }

        public async Task<ApiResponse<OrderDto>> CheckoutAsync(string userId, CreateOrderDto dto)
        {
            // 1. Kiểm tra giỏ hàng của User trong DB
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return ApiResponse<OrderDto>.FailureResult("Giỏ hàng của bạn đang trống, không thể tiến hành đặt hàng.");
            }

            // 2. Xác thực địa chỉ giao hàng
            string shippingAddress;
            string receiverName = dto.ReceiverName?.Trim() ?? string.Empty;
            string receiverPhone = dto.ReceiverPhone?.Trim() ?? string.Empty;

            if (dto.SavedAddressId.HasValue && dto.SavedAddressId.Value > 0)
            {
                var savedAddress = await _context.CustomerAddresses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AddressId == dto.SavedAddressId.Value && a.UserId == userId);

                if (savedAddress != null)
                {
                    receiverName = string.IsNullOrWhiteSpace(receiverName) ? savedAddress.ReceiverName : receiverName;
                    receiverPhone = string.IsNullOrWhiteSpace(receiverPhone) ? savedAddress.ReceiverPhone : receiverPhone;
                    shippingAddress = string.Join(", ", new[] { savedAddress.DetailAddress, savedAddress.Ward, savedAddress.District, savedAddress.Province }.Where(s => !string.IsNullOrWhiteSpace(s)));
                }
                else
                {
                    shippingAddress = BuildShippingAddress(dto.DetailAddress, dto.Ward, dto.District, dto.Province);
                }
            }
            else
            {
                shippingAddress = BuildShippingAddress(dto.DetailAddress, dto.Ward, dto.District, dto.Province);
            }

            if (string.IsNullOrWhiteSpace(shippingAddress))
            {
                return ApiResponse<OrderDto>.FailureResult("Địa chỉ giao hàng không hợp lệ.");
            }

            // Sử dụng Transaction để đảm bảo tính toàn vẹn giữa Tạo Order, Trừ Stock, Cập nhật Voucher và Xóa CartItems
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 3. Kiểm tra số lượng tồn kho và tính tiền SubTotal
                decimal subTotal = 0;
                var orderItemsToCreate = new List<OrderItem>();
                var responseItems = new List<OrderItemDto>();

                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
                    if (product == null || !product.IsActive)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<OrderDto>.FailureResult($"Sản phẩm '{cartItem.Product?.Title ?? cartItem.ProductId.ToString()}' không còn tồn tại hoặc đã ngừng bán.");
                    }

                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return ApiResponse<OrderDto>.FailureResult($"Sản phẩm '{product.Title}' chỉ còn lại {product.StockQuantity} cuốn trong kho, không đủ đáp ứng số lượng bạn đặt ({cartItem.Quantity}).");
                    }

                    // Trừ tồn kho sản phẩm
                    product.StockQuantity -= cartItem.Quantity;

                    // Tính đơn giá sau chiết khấu (nếu có)
                    var unitPrice = product.Price * (1 - product.DiscountPercent / 100m);
                    subTotal += unitPrice * cartItem.Quantity;

                    orderItemsToCreate.Add(new OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = unitPrice
                    });

                    responseItems.Add(new OrderItemDto(
                        product.ProductId,
                        product.Title,
                        product.MainImageUrl,
                        cartItem.Quantity,
                        unitPrice
                    ));
                }

                // 4. Tính toán phí vận chuyển và mã giảm giá
                decimal shippingFee = subTotal >= 250000 ? 0 : 30000; // Miễn phí vận chuyển cho đơn từ 250k
                decimal discountAmount = 0;

                if (!string.IsNullOrWhiteSpace(dto.CouponCode))
                {
                    var voucherResult = await ApplyVoucherAsync(new ApplyVoucherDto(dto.CouponCode, subTotal));
                    if (voucherResult.Success && voucherResult.Data != null && voucherResult.Data.IsValid)
                    {
                        discountAmount = voucherResult.Data.DiscountAmount;

                        // Tăng TimesUsed của coupon
                        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == dto.CouponCode.Trim().ToUpperInvariant());
                        if (coupon != null)
                        {
                            coupon.TimesUsed += 1;
                        }
                    }
                }

                decimal totalAmount = Math.Max(0, subTotal + shippingFee - discountAmount);

                // 5. Sinh mã đơn hàng duy nhất: ORD-yyyyMMdd-XXXX
                var orderCode = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant()}";

                var order = new Order
                {
                    OrderCode = orderCode,
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = (int)OrderStatus.Pending,
                    PaymentMethod = dto.PaymentMethod,
                    IsPaid = false,
                    ReceiverName = receiverName,
                    ReceiverPhone = receiverPhone,
                    ShippingAddress = shippingAddress,
                    SubTotal = subTotal,
                    ShippingFee = shippingFee,
                    DiscountAmount = discountAmount,
                    TotalAmount = totalAmount,
                    Notes = dto.Notes?.Trim(),
                    OrderItems = orderItemsToCreate
                };

                _context.Orders.Add(order);

                // 6. Tự động xóa sạch CartItems của User trong cùng Transaction
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var orderDto = new OrderDto(
                    order.OrderId,
                    order.OrderCode,
                    order.OrderDate,
                    order.Status,
                    GetOrderStatusText((OrderStatus)order.Status),
                    order.PaymentMethod,
                    GetPaymentMethodText((PaymentMethod)order.PaymentMethod),
                    order.IsPaid,
                    order.ReceiverName,
                    order.ReceiverPhone,
                    order.ShippingAddress,
                    order.SubTotal,
                    order.ShippingFee,
                    order.DiscountAmount,
                    order.TotalAmount,
                    order.Notes,
                    null,
                    null,
                    responseItems
                );

                return ApiResponse<OrderDto>.SuccessResult(orderDto, "Đặt hàng thành công!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<OrderDto>.FailureResult($"Quá trình xử lý đặt hàng gặp sự cố: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<OrderListItemDto>>> GetMyOrdersAsync(string userId, string? status = null)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Lọc theo trạng thái đơn hàng nếu có truyền vào
            if (!string.IsNullOrWhiteSpace(status) && !status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(status, out int statusInt))
                {
                    query = query.Where(o => o.Status == statusInt);
                }
                else if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                {
                    query = query.Where(o => o.Status == (int)orderStatus);
                }
            }

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var result = orders.Select(o =>
            {
                var firstItem = o.OrderItems.FirstOrDefault();
                int otherCount = Math.Max(0, o.OrderItems.Count - 1);

                return new OrderListItemDto(
                    o.OrderId,
                    o.OrderCode,
                    o.OrderDate,
                    o.Status,
                    GetOrderStatusText((OrderStatus)o.Status),
                    o.TotalAmount,
                    o.IsPaid,
                    firstItem?.Product?.Title ?? "Sản phẩm",
                    firstItem?.Product?.MainImageUrl,
                    firstItem?.Quantity ?? 1,
                    otherCount,
                    o.ReceiverName,
                    o.ReceiverPhone
                );
            }).ToList();

            return ApiResponse<List<OrderListItemDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(string userId, int orderId)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Shipment!)
                    .ThenInclude(s => s.Provider)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return ApiResponse<OrderDto>.FailureResult("Không tìm thấy đơn hàng hoặc đơn hàng không thuộc về bạn.");
            }

            var items = order.OrderItems.Select(oi => new OrderItemDto(
                oi.ProductId,
                oi.Product?.Title ?? "Sản phẩm",
                oi.Product?.MainImageUrl,
                oi.Quantity,
                oi.UnitPrice
            )).ToList();

            var dto = new OrderDto(
                order.OrderId,
                order.OrderCode,
                order.OrderDate,
                order.Status,
                GetOrderStatusText((OrderStatus)order.Status),
                order.PaymentMethod,
                GetPaymentMethodText((PaymentMethod)order.PaymentMethod),
                order.IsPaid,
                order.ReceiverName,
                order.ReceiverPhone,
                order.ShippingAddress,
                order.SubTotal,
                order.ShippingFee,
                order.DiscountAmount,
                order.TotalAmount,
                order.Notes,
                order.Shipment?.TrackingNumber,
                order.Shipment?.Provider?.ProviderName,
                items
            );

            return ApiResponse<OrderDto>.SuccessResult(dto);
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(string userId, int orderId, string? cancelReason = null)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return ApiResponse<bool>.FailureResult("Không tìm thấy đơn hàng cần hủy.");
            }

            // Chỉ cho phép hủy khi đơn hàng đang ở trạng thái Pending (Chờ duyệt)
            if (order.Status != (int)OrderStatus.Pending)
            {
                return ApiResponse<bool>.FailureResult("Đơn hàng đã được duyệt hoặc đang trong quá trình đóng gói/giao hàng, không thể tự hủy.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Hoàn lại số lượng tồn kho StockQuantity cho từng sản phẩm
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                    }
                }

                order.Status = (int)OrderStatus.Cancelled;
                var reason = string.IsNullOrWhiteSpace(cancelReason) ? "Khách hàng yêu cầu hủy đơn." : cancelReason.Trim();
                order.Notes = string.IsNullOrEmpty(order.Notes) ? $"[Lý do hủy]: {reason}" : $"{order.Notes}\n[Lý do hủy]: {reason}";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<bool>.SuccessResult(true, "Hủy đơn hàng thành công và đã hoàn lại số lượng sản phẩm vào kho.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<bool>.FailureResult($"Hủy đơn hàng thất bại: {ex.Message}");
            }
        }

        private static string BuildShippingAddress(string detail, string ward, string district, string province)
        {
            return string.Join(", ", new[] { detail, ward, district, province }.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        private static string GetOrderStatusText(OrderStatus status) => status switch
        {
            OrderStatus.Pending => "Chờ duyệt",
            OrderStatus.Processing => "Đang đóng gói",
            OrderStatus.Shipping => "Đang giao hàng",
            OrderStatus.Completed => "Hoàn thành",
            OrderStatus.Cancelled => "Đã hủy",
            OrderStatus.Returned => "Đã trả hàng",
            _ => "Không xác định"
        };

        private static string GetPaymentMethodText(PaymentMethod method) => method switch
        {
            PaymentMethod.COD => "Thanh toán khi nhận hàng (COD)",
            PaymentMethod.VNPAY => "VNPAY QR / Thẻ ngân hàng",
            PaymentMethod.Momo => "Ví điện tử MoMo",
            PaymentMethod.BankTransfer => "Chuyển khoản trực tiếp",
            _ => "Khác"
        };
    }
}
