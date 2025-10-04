using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Media.Models.ViewModels
{
    public class IndexVM
    {
        public List<Category> categoryList { get; set; }
        public IEnumerable<Product> products { get; set; }
    }
}
