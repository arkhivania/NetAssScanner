using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public interface IPublicProcessor
    {
        IEnumerable<(AssemblyPublic, Class[])> Extract(string fileName);
    }
}
