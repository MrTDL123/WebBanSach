using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApp.Api.Data;
using WebApp.Api.Entities;
using WebApp.Api.Services.Interfaces;
using WebApp.Shared.Dtos.Common;
using WebApp.Shared.Dtos.Customer.Order;
using WebApp.Shared.Enums;
using WebApp.Shared.Utilities;

namespace WebApp.Api.Services.Implementations
{
    public class CustomerReturnService : ICustomerReturnService
    {
        private readonly AppDbContext _dbContext;

        public CustomerReturnService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<ReturnRequestSummaryDto>> CreateReturnRequestAsync(string userId, CreateReturnRequestDto dto)
        {
            // 1. Kiểm tra đơn hàng có tồn tại và thuộc về user hiện tại không (kèm OrderItems và ReturnRequest)
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.ReturnRequest)
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId && o.UserId == userId);

            if (order == null)
            {
                return ApiResponse<ReturnRequestSummaryDto>.FailureResult("Đơn hàng không tồn tại hoặc không thuộc quyền sở hữu của bạn.");
            }

            // 2. Kiểm tra trạng thái đơn hàng: Phải là Completed (Giao thành công) mới cho phép yêu cầu trả hàng
            if (order.Status != (int)OrderStatus.Completed)
            {
                return ApiResponse<ReturnRequestSummaryDto>.FailureResult("Chỉ các đơn hàng đã được giao thành công (Completed) mới có thể gửi yêu cầu đổi trả.");
            }

            // 3. Kiểm tra đơn hàng đã có yêu cầu đổi trả trước đó chưa
            if (order.ReturnRequest != null)
            {
                return ApiResponse<ReturnRequestSummaryDto>.FailureResult("Đơn hàng này đã có yêu cầu đổi trả được ghi nhận trước đó.");
            }

            // 4. Xử lý tính toán RefundAmount dựa trên các mặt hàng yêu cầu hoàn trả (Items)
            decimal calculatedRefundAmount = 0;
            var returnItemsDetail = new List<string>();

            if (dto.Items != null && dto.Items.Count > 0)
            {
                // Kiểm tra từng sản phẩm yêu cầu hoàn trả có nằm trong đơn hàng và đúng số lượng mua không
                foreach (var returnItem in dto.Items)
                {
                    var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == returnItem.ProductId);
                    if (orderItem == null)
                    {
                        return ApiResponse<ReturnRequestSummaryDto>.FailureResult($"Sản phẩm có mã #{returnItem.ProductId} không nằm trong đơn hàng này.");
                    }

                    if (returnItem.Quantity <= 0)
                    {
                        return ApiResponse<ReturnRequestSummaryDto>.FailureResult($"Số lượng hoàn trả của sản phẩm #{returnItem.ProductId} phải lớn hơn 0.");
                    }

                    if (returnItem.Quantity > orderItem.Quantity)
                    {
                        return ApiResponse<ReturnRequestSummaryDto>.FailureResult($"Số lượng yêu cầu trả ({returnItem.Quantity}) vượt quá số lượng đã mua ({orderItem.Quantity}) của sản phẩm #{returnItem.ProductId}.");
                    }

                    var itemRefund = orderItem.UnitPrice * returnItem.Quantity;
                    calculatedRefundAmount += itemRefund;
                    returnItemsDetail.Add($"ProductId:{returnItem.ProductId} x{returnItem.Quantity} (Đơn giá:{orderItem.UnitPrice:N0}đ)");
                }

                // Tiền hoàn trả cho các sản phẩm không được vượt quá tổng tiền của đơn hàng
                if (calculatedRefundAmount > order.TotalAmount)
                {
                    calculatedRefundAmount = order.TotalAmount;
                }
            }
            else
            {
                // Nếu khách hàng không chỉ định cụ thể item nào thì mặc định hoàn trả toàn bộ giá trị đơn hàng
                calculatedRefundAmount = order.TotalAmount;
            }

            // 5. Xử lý lưu các ảnh minh chứng (Base64) an toàn lên đĩa nếu có
            var savedImageUrls = new List<string>();
            if (dto.EvidenceImagesBase64 != null && dto.EvidenceImagesBase64.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "returns");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                for (int i = 0; i < dto.EvidenceImagesBase64.Count; i++)
                {
                    var base64Str = dto.EvidenceImagesBase64[i];
                    if (string.IsNullOrWhiteSpace(base64Str)) continue;

                    if (base64Str.Contains(","))
                    {
                        base64Str = base64Str.Split(',')[1];
                    }

                    try
                    {
                        var bytes = Convert.FromBase64String(base64Str);
                        if (bytes.Length > 5 * 1024 * 1024) continue;

                        var safeFileName = $"return_{order.OrderId}_{Guid.NewGuid():N}.jpg";
                        var filePath = Path.Combine(uploadsFolder, safeFileName);
                        await File.WriteAllBytesAsync(filePath, bytes);
                        savedImageUrls.Add($"/images/returns/{safeFileName}");
                    }
                    catch
                    {
                        // Bỏ qua nếu có lỗi giải mã một ảnh cụ thể
                    }
                }
            }

            // Chuỗi lưu ProofImageUrl (nhiều ảnh ngăn cách bằng dấu chấm phẩy hoặc JSON array)
            string? proofImageUrl = savedImageUrls.Count > 0 ? string.Join(";", savedImageUrls) : null;

            // Xây dựng nội dung Reason có bao gồm mô tả chi tiết và danh sách item hoàn trả (nếu có)
            var finalReason = dto.Reason;
            if (!string.IsNullOrWhiteSpace(dto.DetailDescription))
            {
                finalReason += $"\n[Mô tả chi tiết]: {dto.DetailDescription}";
            }
            if (returnItemsDetail.Count > 0)
            {
                finalReason += $"\n[Mặt hàng hoàn trả]: {string.Join(", ", returnItemsDetail)}";
            }

            // Cắt ngắn nếu vượt quá 1000 ký tự theo độ dài của DB
            if (finalReason.Length > 1000)
            {
                finalReason = finalReason.Substring(0, 997) + "...";
            }

            // 6. Xử lý thông tin ngân hàng hoàn tiền
            string? bankName = dto.BankName;
            string? bankAccountNumber = dto.BankAccountNumber;
            string? bankAccountHolder = dto.BankAccountHolder;

            if (!string.IsNullOrWhiteSpace(dto.BankAccountInfo))
            {
                var parts = dto.BankAccountInfo.Split(new[] { '-', '|', '/' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToArray();

                if (parts.Length >= 1 && string.IsNullOrWhiteSpace(bankName)) bankName = parts[0];
                if (parts.Length >= 2 && string.IsNullOrWhiteSpace(bankAccountNumber)) bankAccountNumber = parts[1];
                if (parts.Length >= 3 && string.IsNullOrWhiteSpace(bankAccountHolder)) bankAccountHolder = parts[2];

                // Nếu không phân tách được nhiều phần thì gán trực tiếp vào BankName
                if (string.IsNullOrWhiteSpace(bankName))
                {
                    bankName = dto.BankAccountInfo.Trim();
                }
            }

            // Tạo thực thể ReturnRequest
            var returnRequest = new ReturnRequest
            {
                OrderId = order.OrderId,
                CustomerId = userId,
                Reason = finalReason,
                ProofImageUrl = proofImageUrl,
                Status = (int)ReturnStatus.PendingApproval,
                BankName = bankName,
                BankAccountNumber = bankAccountNumber,
                BankAccountHolder = bankAccountHolder,
                RefundAmount = calculatedRefundAmount,
                RequestedAt = DateTime.UtcNow
            };

            _dbContext.ReturnRequests.Add(returnRequest);
            await _dbContext.SaveChangesAsync();

            var summary = new ReturnRequestSummaryDto(
                ReturnRequestId: returnRequest.ReturnRequestId,
                OrderId: returnRequest.OrderId,
                OrderCode: order.OrderCode,
                Status: (ReturnStatus)returnRequest.Status,
                StatusText: GetReturnStatusText((ReturnStatus)returnRequest.Status),
                Reason: returnRequest.Reason,
                BankAccountInfo: FormatBankAccountInfo(returnRequest.BankName, returnRequest.BankAccountNumber, returnRequest.BankAccountHolder),
                RefundAmount: returnRequest.RefundAmount,
                RequestedAt: returnRequest.RequestedAt,
                ProofImageUrl: returnRequest.ProofImageUrl,
                ProcessedAt: returnRequest.ProcessedAt
            );

            return ApiResponse<ReturnRequestSummaryDto>.SuccessResult(summary, "Gửi yêu cầu trả hàng thành công!");
        }

        public async Task<ApiResponse<List<ReturnRequestSummaryDto>>> GetMyReturnRequestsAsync(string userId)
        {
            var returnRequests = await _dbContext.ReturnRequests
                .AsNoTracking()
                .Where(r => r.CustomerId == userId)
                .Include(r => r.Order)
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new ReturnRequestSummaryDto(
                    r.ReturnRequestId,
                    r.OrderId,
                    r.Order.OrderCode,
                    (ReturnStatus)r.Status,
                    GetReturnStatusText((ReturnStatus)r.Status),
                    r.Reason,
                    FormatBankAccountInfo(r.BankName, r.BankAccountNumber, r.BankAccountHolder),
                    r.RefundAmount,
                    r.RequestedAt,
                    r.ProofImageUrl,
                    r.ProcessedAt
                ))
                .ToListAsync();

            return ApiResponse<List<ReturnRequestSummaryDto>>.SuccessResult(returnRequests, "Lấy danh sách yêu cầu trả hàng thành công.");
        }

        private static string GetReturnStatusText(ReturnStatus status) => status switch
        {
            ReturnStatus.PendingApproval => "Chờ duyệt",
            ReturnStatus.Approved => "Đã duyệt",
            ReturnStatus.ItemReceived => "Đã nhận hàng",
            ReturnStatus.Refunded => "Đã hoàn tiền",
            ReturnStatus.Rejected => "Từ chối",
            _ => "Không xác định"
        };

        private static string? FormatBankAccountInfo(string? bankName, string? accountNumber, string? holder)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(bankName)) parts.Add(bankName);
            if (!string.IsNullOrWhiteSpace(accountNumber)) parts.Add(accountNumber);
            if (!string.IsNullOrWhiteSpace(holder)) parts.Add(holder);

            return parts.Count > 0 ? string.Join(" - ", parts) : null;
        }
    }
}
