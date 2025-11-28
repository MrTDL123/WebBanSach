using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Media.Models
{
    public class TacGia
    {
        [Key]
        public int MaTacGia { get; set; }

        [Required(ErrorMessage = "Tên tác giả là bắt buộc")]
        [DisplayName("Tên tác giả")]
        [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự")]
        public string TenTG { get; set; } = string.Empty; // ✅ KHỞI TẠO MẶC ĐỊNH

        [DisplayName("Tiểu sử")]
        [StringLength(1000, ErrorMessage = "Tiểu sử không được vượt quá 1000 ký tự")]
        public string? TieuSu { get; set; }

        [DisplayName("Quốc tịch")]
        [StringLength(50, ErrorMessage = "Quốc tịch không được vượt quá 50 ký tự")]
        public string? QuocTich { get; set; }

        public virtual ICollection<Sach>? Saches { get; set; }

        // ✅ THÊM CONSTRUCTOR
        public TacGia()
        {
            TenTG = string.Empty;
        }
    }
}