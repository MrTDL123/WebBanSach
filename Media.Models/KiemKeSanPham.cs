using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class KiemKeSanPham
    {
        [Key]
        [DisplayName("Mã kiểm kê")]
        public int MaKiemKe { get; set; }
        [DataType(DataType.Date)]
        public DateTime? NgayKiemKe { get; set; }
        public string GhiChu { get; set; }


        //NAVIGATION PROPERTIES
        public int? MaNhanVienKiemKe { get; set; }
        [ForeignKey("MaNhanVienKiemKe")]
        public NhanVien NhanVienKiemKe { get; set; }
        public ICollection<ChiTietKiemKe> ChiTietKiemKes { get; set; }
    }
}
