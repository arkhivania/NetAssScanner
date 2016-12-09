using System;
using System.Collections.Generic;
using System.Linq;

namespace Nailhang.Web.Utils
{
    public static class StringUtils
    {
        public static string GetNamespace(string typeName)
        {
            var splited = typeName.Split('.');
            return splited.Take(splited.Length - 1)
                .Aggregate((a, b) => a + "." + b);
        }
    }
}