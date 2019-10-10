using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public interface IPublicProcessor
    {
        IEnumerable<AssemblyPublic> Extract(string fileName);
    }
}
