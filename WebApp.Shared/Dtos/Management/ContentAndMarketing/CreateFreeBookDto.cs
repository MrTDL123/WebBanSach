using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    public record CreateFreeBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string? Source { get; set; }
        public bool IsActive { get; set; } = true;
        // Xử lý ảnh bìa
        public string? CoverImageBase64 { get; set; }
        public string? CoverImageFileName { get; set; }
        public CreateFreeBookDto() { }
    }
}
