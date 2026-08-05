namespace WebApp.Shared.Enums
{
    /// <summary>
    /// Trạng thái vòng đời của một Ticket Hỗ trợ Khách hàng (Support Ticket).
    /// </summary>
    public enum TicketStatus
    {
        /// <summary>Ticket mới được tạo, chưa có nhân viên CSKH tiếp nhận</summary>
        Open = 0,

        /// <summary>Đã có nhân viên CSKH tiếp nhận và đang xử lý</summary>
        InProgress = 1,

        /// <summary>Vấn đề đã được giải quyết, chờ khách hàng xác nhận</summary>
        Resolved = 2,

        /// <summary>Ticket đã đóng (khách xác nhận OK hoặc tự động đóng sau 7 ngày)</summary>
        Closed = 3
    }

    /// <summary>
    /// Trạng thái của Vận đơn Giao hàng (Shipment) do đơn vị 3PL cập nhật.
    /// </summary>
    public enum ShipmentStatus
    {
        /// <summary>Đang chờ nhân viên kho đóng gói và bàn giao cho 3PL đến lấy</summary>
        WaitingForPickup = 0,

        /// <summary>3PL đã lấy hàng, đang trên đường giao đến khách</summary>
        InTransit = 1,

        /// <summary>Giao thành công, khách đã nhận hàng</summary>
        Delivered = 2,

        /// <summary>Giao thất bại (khách không nghe máy, địa chỉ sai...)</summary>
        DeliveryFailed = 3,

        /// <summary>Hàng đang được chuyển hoàn về kho sau khi giao thất bại</summary>
        Returning = 4,

        /// <summary>Hàng hoàn đã về đến kho</summary>
        ReturnedToWarehouse = 5
    }

    /// <summary>
    /// Tình trạng vật lý của sách (dùng cho Tủ sách 0 đồng).
    /// </summary>
    public enum BookCondition
    {
        /// <summary>Sách mới hoàn toàn, chưa qua sử dụng</summary>
        New = 0,

        /// <summary>Sách đã đọc nhưng còn tốt, không có dấu ghi chép hay hỏng hóc</summary>
        Good = 1,

        /// <summary>Sách còn đọc được, có thể có vài vết bút chì hoặc cũ nhẹ</summary>
        Fair = 2
    }

    /// <summary>
    /// Trạng thái xử lý Phiếu Thỉnh Sách / Tặng Sách (Free Book Request/Donation).
    /// </summary>
    public enum FreeBookRequestStatus
    {
        /// <summary>Khách vừa gửi yêu cầu, chờ nhân viên duyệt</summary>
        Pending = 0,

        /// <summary>Đã duyệt, sách đang được chuẩn bị giao / 3PL đang đến lấy</summary>
        Approved = 1,

        /// <summary>Đang trong quá trình vận chuyển</summary>
        Shipping = 2,

        /// <summary>Hoàn tất (Khách đã nhận sách / Kho đã nhận sách tặng)</summary>
        Completed = 3,

        /// <summary>Bị từ chối (lý do không chính đáng, sách không đủ tiêu chuẩn...)</summary>
        Rejected = 4
    }
}
