using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public enum GioiTinh : byte
    {
        [Description("Nữ")]
        Nu = 0,

        [Description("Nam")]
        Nam = 1,

        [Description("Khác")]
        Khac = 2
    }
}