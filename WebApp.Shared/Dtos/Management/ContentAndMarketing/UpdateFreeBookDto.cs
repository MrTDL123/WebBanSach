using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    public record UpdateFreeBookDto
    {
        public int FreeBookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string? Source { get; set; }
        public int AvailableQty { get; set; } // Số lượng hiện có
        public bool IsActive { get; set; }
        // Ảnh bìa hiện tại & ảnh mới thay thế (nếu có)
        public string? CoverImageUrl { get; set; }
        public string? CoverImageBase64 { get; set; }
        public string? CoverImageFileName { get; set; }
        public UpdateFreeBookDto() { }
    }
}
