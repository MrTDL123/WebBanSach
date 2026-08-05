namespace WebApp.Shared.Enums
{
    /// <summary>
    /// Trạng thái vòng đời của một Đơn hàng.
    /// Luồng chuẩn: Pending → Processing → Shipping → Completed
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>Chờ nhân viên [Order & Logistics] xác nhận duyệt đơn</summary>
        Pending = 0,

        /// <summary>Đã duyệt, đang chờ [Warehouse] đóng gói</summary>
        Processing = 1,

        /// <summary>Đã bàn giao cho đơn vị 3PL (GHN/GHTK...), đang trên đường giao</summary>
        Shipping = 2,

        /// <summary>Giao thành công, khách đã nhận hàng và thanh toán COD</summary>
        Completed = 3,

        /// <summary>Đơn bị hủy (bởi khách hàng hoặc nhân viên)</summary>
        Cancelled = 4,

        /// <summary>Khách đã gửi yêu cầu trả hàng và được duyệt</summary>
        Returned = 5
    }
}
