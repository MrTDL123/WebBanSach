using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public class KhachHang
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string HoTen { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }


        //NAVIGATION PROPERTIES
        public string TaiKhoanId { get; set; }
        [ForeignKey("TaiKhoanId")]
        public TaiKhoan TaiKhoan { get; set; }

        public ICollection<DonHang> DonHangs { get; set; }
    }
}
