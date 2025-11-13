using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Media.Models.ViewModels
{
    public class IndexVM
    {
        public IEnumerable<ChuDe>? DanhSachChuDe { get; set; }
        public IEnumerable<Sach>? SachBanChay { get; set; }
    }
}
