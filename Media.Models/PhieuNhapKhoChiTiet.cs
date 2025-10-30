using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class PhieuNhapKhoChiTiet
    {
        [Key, Column(Order = 0)]
        public int MaPhieuNhapKho { get; set; }

        [Key, Column(Order = 1)]
        public int MaSach { get; set; }

        public int SoLuong { get; set; }
        public decimal? DonGia { get; set; }



        //NAVIGATION PROPERTIES
        [ForeignKey("MaPhieuNhapKho")]
        public PhieuNhapKho PhieuNhapKho { get; set; }

        [ForeignKey("MaSach")]
        public Sach Sach { get; set; }
    }
}
