namespace WebApp.Shared.Enums
{
    /// <summary>
    /// Phương thức thanh toán khi đặt hàng.
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>Thanh toán tiền mặt khi nhận hàng (Cash On Delivery)</summary>
        COD = 0,

        /// <summary>Thanh toán qua cổng VNPAY (ATM/Visa/QR)</summary>
        VNPAY = 1,

        /// <summary>Thanh toán qua ví điện tử MoMo</summary>
        Momo = 2,

        /// <summary>Chuyển khoản ngân hàng trực tiếp</summary>
        BankTransfer = 3
    }
}
