using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public class ChiTietKiemKe
    {
        [Key]
        public int MaChiTietKiemKe { get; set; }

        [Required]
        [DisplayName("Số lượng thực tế")]
        public int SoLuongThucTe { get; set; }


        //NAVIGATION PROPERTIES
        public int MaSach { get; set; }
        public int MaKiemKeSanPham { get; set; }

        [ForeignKey("MaSach")]
        public Sach Sach { get; set; }
        [ForeignKey("MaKiemKeSanPham")]
        public KiemKeSanPham KiemKeSanPham { get; set; }
    }
}
