using Nailhang.IndexBase.PublicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Base
{
    public interface ISearchItem
    {
        string FullClassName { get; }
        string Namespace { get; }
        bool IsPublic { get; }
        string ClassName { get; }
        AssemblyPublic Assembly { get; }
    }
}
