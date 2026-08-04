namespace WebBanSach.Shared.Dtos.AdminClient.FinanceAndAccountant
{
    // ===== ĐỐI SOÁT DÒNG TIỀN COD (Finance Module - Admin) =====
    // Component dùng: DoiSoatCOD.razor (Admin - Finance & Accountant)
    // Định kỳ 3PL chuyển tiền COD về, nhân viên kế toán dùng tính năng này để đối soát

    // API trả về để hiển thị bảng đối soát
    public record CodReconciliationItemDto(
        int OrderId,
        string OrderCode,
        string CustomerName,
        string ShippingProviderName,
        string TrackingNumber,
        decimal CodAmount,            // Số tiền COD 3PL đã thu của khách
        DateTime DeliveredAt,
        bool IsReconciled,            // Đã được nhân viên kế toán xác nhận khớp số chưa
        DateTime? ReconciledAt
    );
}
