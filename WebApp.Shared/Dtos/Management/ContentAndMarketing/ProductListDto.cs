using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    public record ProductListDto(
        int ProductId,
        string Title,
        string? MainImageUrl,
        string CategoryName,
        string AuthorName,
        string PublisherName,
        decimal Price,
        decimal DiscountPercent,
        decimal FinalPrice,
        int StockQuantity,
        bool IsActive,
        DateTime CreatedAt
    );
}
