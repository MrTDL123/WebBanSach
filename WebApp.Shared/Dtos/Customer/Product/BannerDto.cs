namespace WebApp.Shared.Dtos.CustomerDtos.Product
{
    /// <summary>
    /// Thông tin Banner quảng cáo trên trang chủ (Web & Mobile)
    /// </summary>
    public record BannerDto(
        int BannerId,
        string Title,
        string ImageUrl,
        string? TargetUrl,
        int DisplayOrder
    );
}
