using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Media.Models
{
    public class TacGia
    {
        [Key]
        public int MaTacGia { get; set; }

        [Required]
        [DisplayName("Tên tác giả")]
        [StringLength(50)]
        public string TenTG { get; set; }

        public virtual ICollection<Sach> Saches { get; set; }
        [DisplayName("Tiểu sử")]
        public string? TieuSu { get; set; }

        [DisplayName("Quốc tịch")]
        public string? QuocTich { get; set; }
    }
}