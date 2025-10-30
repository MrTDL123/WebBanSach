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
        public int MaKhachHang { get; set; }
        [Required]
        [StringLength(30)]
        public string HoTen { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }


        //NAVIGATION PROPERTIES
        public string MaTaiKhoan { get; set; }
        [ForeignKey("MaTaiKhoan")]
        public TaiKhoan TaiKhoan { get; set; }

        public ICollection<DonHang> DonHangs { get; set; }
        public ICollection<PhanHoiKhachHang> PhanHoiKhachHangs{ get; set; }

    }
}
