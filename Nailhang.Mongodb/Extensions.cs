using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb
{
    static class Extensions
    {
        static MD5 md5 = MD5.Create();

        public static Guid GenerateGuid(this string uniqueName)
        {
            lock (md5)
                return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(uniqueName)));
        }
    }
}
