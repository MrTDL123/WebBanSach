using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    [NotMapped] // For TPH we won't create separate table; discriminator handled in DbContext
    public class QuanLy : NhanVien
    {
        [DisplayName("Cấp bậc")]
        public string CapBac { get; set; }
    }
}
