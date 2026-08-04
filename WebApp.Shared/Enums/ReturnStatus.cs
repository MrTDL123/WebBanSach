namespace WebBanSach.Shared.Enums
{
    /// <summary>
    /// Trạng thái vòng đời của một Phiếu Trả hàng / Hoàn tiền.
    /// Luồng chuẩn: PendingApproval → Approved → ItemReceived → Refunded
    /// </summary>
    public enum ReturnStatus
    {
        /// <summary>Khách vừa gửi yêu cầu, đang chờ CSKH xem xét và phê duyệt</summary>
        PendingApproval = 0,

        /// <summary>CSKH đã duyệt, đang chờ 3PL đến nhà khách lấy hàng về</summary>
        Approved = 1,

        /// <summary>Kho đã nhận lại hàng từ khách hàng, đang kiểm tra tình trạng</summary>
        ItemReceived = 2,

        /// <summary>Kế toán đã xác nhận hoàn tiền thành công cho khách hàng</summary>
        Refunded = 3,

        /// <summary>Yêu cầu trả hàng bị từ chối (lý do không hợp lệ, quá thời hạn...)</summary>
        Rejected = 4
    }
}
