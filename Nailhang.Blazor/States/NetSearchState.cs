using Nailhang.Display.NetPublicSearch.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Blazor.States
{
    public class NetSearchState
    {
        public string[] Namespaces { get; set; }
        public string SelectedNamespace { get; set; }
        public bool ShowOnlyPublic { get; set; }
        public ISearchItem[] DisplayItems { get; set; }
    }
}
