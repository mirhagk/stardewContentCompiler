using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCompiler
{
    static class Extensions
    {
        public static int? GetInt(this string value)
        {
            int result;
            if (int.TryParse(value, out result))
                return result;
            return null;
        }
    }
}
