using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    [Table("PhieuNhapKho")]
    public class PhieuNhapKho
    {
        [Key]
        [DisplayName("Số phiếu")]
        public int MaPhieuNhapKho { get; set; }

        
        [DataType(DataType.Date)]
        [DisplayName("Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [DisplayName("Ghi chú")]
        public string GhiChu { get; set; }

        //NAVIGATION PROPERTIES
        public int? MaNhanVienNhapKho { get; set; }
        [ForeignKey("MaNhanVienNhapKho")]
        [DisplayName("Nhân viên tạo phiếu")]
        public NhanVien NhanVienNhapKho { get; set; }

        public ICollection<PhieuNhapKhoChiTiet> PhieuNhapKhoChiTiets { get; set; }
    }
}
