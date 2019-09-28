using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Display.MD5Cache
{
    public class MD5NonBlockingCache : IMD5Cache
    {
        readonly ConcurrentDictionary<string, Guid> md5Dict = new ConcurrentDictionary<string, Guid>();

        static Guid MakeGuidFromString(string content)
        {
            using (var md5 = MD5.Create())
                return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(content)));
        }

        public Guid ToMD5(string parameter)
        {
            Guid res;
            if (md5Dict.TryGetValue(parameter, out res))
                return res;
            res = MakeGuidFromString(parameter);
            md5Dict.TryAdd(parameter, res);
            return res;
        }
    }
}
