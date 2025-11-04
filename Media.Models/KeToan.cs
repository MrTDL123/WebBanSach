using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    //[NotMapped]
    public class KeToan : NhanVien
    {
        [DisplayName("Bộ phận phụ trách")]
        public string BoPhanPhuTrach { get; set; }
    }
}