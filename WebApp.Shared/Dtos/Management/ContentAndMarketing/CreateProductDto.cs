namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    // Component dùng: ThemSach.razor (Admin - Content & Marketing)
    public record CreateProductDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercent { get; set; }
        public int StockQuantity { get; set; }
        public string? SupplierName { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
        public int PublisherId { get; set; }
        public string? MainImageBase64 { get; set; } = null;
        public string? MainImageFileName { get; set; } = null;
        public List<ProductGalleryItemDto> GalleryImages { get; set; } = new();

        public CreateProductDto() { }
    }
}
