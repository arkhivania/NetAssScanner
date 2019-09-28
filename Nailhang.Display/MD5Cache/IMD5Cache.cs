using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Display.MD5Cache
{
    public interface IMD5Cache
    {
        Guid ToMD5(string parameter);
    }
}
