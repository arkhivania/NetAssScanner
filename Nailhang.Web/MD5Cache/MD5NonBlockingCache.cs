using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Web.MD5Cache
{
    class MD5NonBlockingCache : IMD5Cache
    {
        readonly ConcurrentDictionary<string, Guid> md5Dict = new ConcurrentDictionary<string, Guid>();

        public Guid ToMD5(string parameter)
        {
            Guid res;
            if (md5Dict.TryGetValue(parameter, out res))
                return res;
            res = parameter.ToMD5();
            md5Dict.TryAdd(parameter, res);
            return res;
        }
    }
}
