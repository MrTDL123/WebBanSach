using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Meida.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebBanSach.ViewComponents
{
    public class ChuDeMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ChuDeMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<ChuDe> listChuDe = _context.ChuDes;

            IEnumerable<ChuDe> chudeTree = await BuildChuDeTree(listChuDe, null);

            return View(chudeTree);
        }

        private async Task<List<ChuDe>> BuildChuDeTree(IEnumerable<ChuDe> list, int? parentID)
        {
            List<ChuDe> children = list
                                    .Where(cd => cd.ParentId == parentID)
                                    .ToList();

            foreach (ChuDe child in children)
            { 
                child.Children = await BuildChuDeTree(list, child.MaChuDe);
            }

            return children;
        }
    }
}
