using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebBanSach.ViewComponents
{
    public class ChuDeMenuViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unit;

        public ChuDeMenuViewComponent(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<ChuDe> listChuDe = await _unit.ChuDes.GetAllReadOnlyAsync();

            IEnumerable<ChuDe> chudeTree = BuildChuDeTree(listChuDe, null);

            return View(chudeTree);
        }

        private List<ChuDe> BuildChuDeTree(IEnumerable<ChuDe> list, int? parentID)
        {
            List<ChuDe> children = list
                                    .Where(cd => cd.ParentId == parentID)
                                    .ToList();

            foreach (ChuDe child in children)
            {
                child.Children = BuildChuDeTree(list, child.MaChuDe);
            }

            return children;
        }
    }
}
