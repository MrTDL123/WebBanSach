using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public class YeuThich
    {
        [Key]
        public int MaYeuThich { get; set; }

        public int MaKhachHang { get; set; } 

        public int MaSach { get; set; }     

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // NAVIGATION PROPERTIES
        [ForeignKey("MaKhachHang")]
        public virtual KhachHang KhachHang { get; set; }

        [ForeignKey("MaSach")]
        public virtual Sach Sach { get; set; }
    }
}
