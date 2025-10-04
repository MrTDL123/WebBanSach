using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Media.Models
{
    public class Category
    {
        #nullable disable
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Tên thể loại")]
        public string Name { get; set; }
        [DisplayName("Số thứ tự")]
        [Range(1, 100, ErrorMessage = "Display Order cần phải trong tầm từ 1-100")]

        public int DisplayOrder { get; set; }
    }
}
