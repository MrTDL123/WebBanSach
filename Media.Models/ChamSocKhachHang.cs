using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    [Table("ChamSocKhachHang")]
    public class ChamSocKhachHang
    {
        [Key]
        [DisplayName("Mã chăm sóc")]
        public int MaChamSoc { get; set; }

        [DisplayName("Nội dung")]
        public string NoiDung { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Ngày chăm sóc")]
        public DateTime? NgayChamSoc { get; set; }



        //NAVIGATION PROPERTIES
        public int? MaNhanVienChamSoc { get; set; }
        [ForeignKey("MaNhanVienChamSoc")]
        public NhanVien NhanVienChamSoc { get; set; }
    }
}
