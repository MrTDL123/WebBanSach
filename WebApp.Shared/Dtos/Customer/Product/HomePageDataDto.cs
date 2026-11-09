using System.Collections.Generic;
using WebApp.Shared.Dtos.CustomerDtos.FreeBook;
using WebApp.Shared.Dtos.Management.ContentAndMarketing;

namespace WebApp.Shared.Dtos.CustomerDtos.Product
{
    /// <summary>
    /// DTO tổng hợp toàn bộ dữ liệu hiển thị trang chủ (Web Home.razor & Mobile HomeActivity)
    /// Giảm thiểu round-trip mạng từ nhiều API calls xuống còn 1 call duy nhất.
    /// </summary>
    public record HomePageDataDto(
        List<BannerDto> Banners,
        List<CategoryDto> Categories,
        List<ProductListItemDto> NewProducts,
        List<ProductListItemDto> BestSellerProducts,
        List<FreeBookDto> FreeBooks
    );
}
