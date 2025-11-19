using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Utility
{
    public static class HtmlHelpers
    {
        public static string Selected(this string value, string compareValue)
        {
            return value == compareValue ? "selected" : "";
        }
    }
}
