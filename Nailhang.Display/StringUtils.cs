using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Nailhang.Display
{
    public static class StringUtils
    {
        public static Guid ToMD5(this string content)
        {
            using (var md5 = MD5.Create())
                return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(content)));
        }

        public static string GetNamespace(string typeName)
        {
            var splited = typeName.Split('.');
            return splited.Take(splited.Length - 1)
                .Aggregate((a, b) => a + "." + b);
        }
    }
}