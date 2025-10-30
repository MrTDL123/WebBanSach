using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public class TacGia
    {
        [Key]
        public int MaTacGia { get; set; }
        [Required]
        [DisplayName("Tên tác giả")]
        public string TenTG { get; set; }
    }
}
