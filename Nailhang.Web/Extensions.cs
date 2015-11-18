using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Nailhang.Web
{
    public static class Extensions
    {
        public static Guid ToMD5(this string content)
        {
            using (var md5 = MD5.Create())
                return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(content)));
        }

        public static string GetClassName(this string fullName)
        {
            return fullName.Split('.').Last();
        }
    }
}