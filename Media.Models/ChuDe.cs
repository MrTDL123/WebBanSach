using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class ChuDe
    {
        #nullable disable
        [Key]
        public int MaChuDe { get; set; }
        [Required]
        [MaxLength(255)]
        [DisplayName("Tên thể loại")]
        public string TenChuDe { get; set; }
        public int? ParentId { get; set; }

        public string Slug { get; set; }
        public string FullPath { get; set; }

        public IEnumerable<ChuDe> Children { get; set; } = new List<ChuDe>();
    }
}