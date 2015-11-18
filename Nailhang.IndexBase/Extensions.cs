using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.IndexBase
{
    public static class Extensions
    {
        public static string ToNamespace(this string fullName)
        {
            var split = fullName.Split(separator: new char[] { '.' });
            return split.Take(split.Length - 1)
                .Aggregate((a, b) => a + "." + b);
        }
    }
}
