using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public class NhaXuatBan
    {
        [Key]
        public int MaNhaXuatBan { get; set; }
        [Required]
        [DisplayName("Tên nhà xuất bản")]
        public string TenNXB { get; set; }
    }
}
