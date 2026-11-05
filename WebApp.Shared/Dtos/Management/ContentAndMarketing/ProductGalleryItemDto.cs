using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Shared.Dtos.Management.ContentAndMarketing
{
    public record ProductGalleryItemDto
    {
        public int ImageId { get; set; } // = 0 nếu là ảnh mới tải lên, > 0 nếu là ảnh cũ trong DB
        public string? ImageUrl { get; set; } // Đường dẫn ảnh cũ (nếu có)
        public string? Base64Data { get; set; } // Dữ liệu Base64 nếu người dùng chọn file mới
        public string? FileName { get; set; }
        public int DisplayOrder { get; set; } // Thứ tự hiển thị (1, 2, 3...)
        public bool IsDeleted { get; set; } // Dùng cho Update: Đánh dấu ảnh phụ nào bị bấm Xóa
    }
}
