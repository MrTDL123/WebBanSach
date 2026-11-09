namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: SuaSach.razor (Admin - Content & Marketing)
    public record UpdateProductDto
    {
        public int ProductId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercent { get; set; }
        public int StockQuantity { get; set; }
        public string? SupplierName { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
        public int PublisherId { get; set; }
        public bool IsActive { get; set; }
        public string? MainImageUrl { get; set; } = string.Empty;
        public string? MainImageBase64 { get; set; } = string.Empty;
        public string? MainImageFileName { get; set; } = string.Empty;
        public List<ProductGalleryItemDto> GalleryImages { get; set; } = new();

        public UpdateProductDto() { }
    }
}
