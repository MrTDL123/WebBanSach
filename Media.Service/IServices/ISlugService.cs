using Media.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Service.IServices
{
    public interface ISlugService
    {
        string GenerateSlug(string text);
        ChuDe GetChuDeByPath(string path);
        string GetFullPath(ChuDe chuDe);
    }
}
