namespace WebApp.Shared.Constants
{
    /// <summary>
    /// Danh sách hằng số định danh Feature Flags (Cờ tính năng).
    /// </summary>
    public static class FeatureFlags
    {
        /// <summary>
        /// Bật/tắt khối Flash Sale và đồng hồ đếm ngược trên trang chủ.
        /// </summary>
        public const string FlashSaleSection = "FlashSaleSection";

        /// <summary>
        /// Bật/tắt slider/carousel quảng cáo sự kiện ở đầu trang.
        /// </summary>
        public const string HeroBannerSlider = "HeroBannerSlider";

        /// <summary>
        /// Bật/tắt chuyên mục tủ sách khuyến học / sách 0đ.
        /// </summary>
        public const string FreeBooksPromotion = "FreeBooksPromotion";

        /// <summary>
        /// Bật/tắt giao diện theo mùa (Tết, Giáng Sinh, Back to School, Black Friday...).
        /// </summary>
        public const string SeasonalTheme = "SeasonalTheme";
    }
}
