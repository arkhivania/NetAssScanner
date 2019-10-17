using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Base
{
    public struct NamespaceInfo
    {
        public string Namespace { get; set; }
        public int Levels { get; set; }        

        public IEnumerable<NamespaceInfo> AllNamespaces 
        {
            get 
            {
                var parts = Namespace.Split('.');
                for (int i = 1; i <= parts.Length; ++i)
                    yield return new NamespaceInfo { Namespace = string.Join(".", parts.Take(i)), Levels = i };
            }
        }
    }
}
