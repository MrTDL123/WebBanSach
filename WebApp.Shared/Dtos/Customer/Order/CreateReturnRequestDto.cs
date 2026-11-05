using System;
using System.Collections.Generic;
using WebApp.Shared.Enums;

namespace WebApp.Shared.Dtos.Customer.Order
{
    /// <summary>
    /// Thông tin mặt hàng yêu cầu trả lại
    /// </summary>
    public record ReturnItemInputDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }

        public ReturnItemInputDto() { }

        public ReturnItemInputDto(int productId, int quantity, string? note = null)
        {
            ProductId = productId;
            Quantity = quantity;
            Note = note;
        }
    }

    /// <summary>
    /// DTO tạo yêu cầu trả hàng thống nhất cho cả Web và Mobile
    /// </summary>
    public record CreateReturnRequestDto
    {
        public int OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? DetailDescription { get; set; }

        // Thông tin tài khoản ngân hàng để hoàn tiền (hỗ trợ nhập nhanh qua BankAccountInfo hoặc các trường chi tiết)
        public string? BankAccountInfo { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountHolder { get; set; }

        // Danh sách mặt hàng khách muốn hoàn trả (nếu để trống/null thì mặc định hoàn trả toàn bộ đơn hàng)
        public List<ReturnItemInputDto>? Items { get; set; } = new();

        // Danh sách ảnh minh chứng (Base64) tối đa 5 ảnh, mỗi ảnh tối đa 5MB
        public List<string>? EvidenceImagesBase64 { get; set; } = new();
        public List<string>? EvidenceImageFileNames { get; set; } = new();

        public CreateReturnRequestDto() { }

        public CreateReturnRequestDto(
            int orderId,
            string reason,
            string? detailDescription = null,
            string? bankName = null,
            string? bankAccountNumber = null,
            string? bankAccountHolder = null,
            string? bankAccountInfo = null,
            List<ReturnItemInputDto>? items = null,
            List<string>? evidenceImagesBase64 = null,
            List<string>? evidenceImageFileNames = null)
        {
            OrderId = orderId;
            Reason = reason;
            DetailDescription = detailDescription;
            BankName = bankName;
            BankAccountNumber = bankAccountNumber;
            BankAccountHolder = bankAccountHolder;
            BankAccountInfo = bankAccountInfo;
            Items = items ?? new();
            EvidenceImagesBase64 = evidenceImagesBase64 ?? new();
            EvidenceImageFileNames = evidenceImageFileNames ?? new();
        }
    }

    /// <summary>
    /// DTO thông tin phiếu trả hàng trả về cho Client (hỗ trợ cả Web và Mobile)
    /// </summary>
    public record ReturnRequestSummaryDto(
        int ReturnRequestId,
        int OrderId,
        string OrderCode,
        ReturnStatus Status,
        string StatusText,
        string Reason,
        string? BankAccountInfo,
        decimal RefundAmount,
        DateTime RequestedAt,
        string? ProofImageUrl = null,
        DateTime? ProcessedAt = null
    );
}
