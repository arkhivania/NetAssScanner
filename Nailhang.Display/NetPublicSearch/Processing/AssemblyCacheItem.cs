using Nailhang.IndexBase.PublicApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Processing
{
    struct AssemblyCacheItem
    {
        public AssemblyPublic Assembly { get; set; }
        public Class[] Classes { get; set; }
    }
}
