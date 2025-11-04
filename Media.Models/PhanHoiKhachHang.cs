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
    public class PhanHoiKhachHang
    {
        [Key]
        [DisplayName("Mã phản hồi")]
        public int MaPhanHoiKhachHang { get; set; }
        [DisplayName("Nội dung")]
        public string NoiDung { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [StringLength(100)]
        [DisplayName("Loại phản hồi")]
        public string LoaiPhanHoi { get; set; }


        //NAVIGATION PROPERTIES
        public int MaKhachHang { get; set; }
        [ForeignKey("MaKhachHang")]
        public KhachHang KhachHang { get; set; }
    }
}
