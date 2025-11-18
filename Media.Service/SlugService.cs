using Media.DataAccess.Repository.IRepository;
using Media.Models;
using Media.Utility;
using Media.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Media.Service
{
    public class SlugService : ISlugService
    {
        private readonly IUnitOfWork _unit;

        public SlugService (IUnitOfWork unit)
        {
            _unit = unit;
        }

        public string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Chuyển về chữ thường
            text = text.ToLowerInvariant();

            // Bỏ dấu tiếng Việt
            text = ChuanHoaStringHelper.XoaDauTiengViet(text);

            // Thay khoảng trắng bằng dấu gạch ngang
            text = Regex.Replace(text, @"\s+", "-");

            // Bỏ ký tự đặc biệt
            text = Regex.Replace(text, @"[^a-z0-9\-]", "");

            // Bỏ dấu gạch ngang thừa
            text = Regex.Replace(text, @"-+", "-");

            // Trim đầu cuối
            text = text.Trim('-');

            return text;
        }

        public ChuDe GetChuDeByPath(string path)
        {
            return _unit.ChuDes.Get(cd => cd.FullPath == path);
        }

        public string GetFullPath(ChuDe chuDe)
        {
            List<string> pathSegments = new List<string>();
            ChuDe current = chuDe;

            while (current != null)
            {
                pathSegments.Insert(0, current.Slug);
                current = _unit.ChuDes.Get(cd => cd.MaChuDe == current.ParentId);
            }

            return string.Join("/", pathSegments);
        }
    }
}
