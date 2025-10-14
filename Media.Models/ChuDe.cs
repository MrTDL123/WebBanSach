using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Media.Models
{
    public class ChuDe
    {
        #nullable disable
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Tên thể loại")]
        public string TenChuDe { get; set; }
    }
}
