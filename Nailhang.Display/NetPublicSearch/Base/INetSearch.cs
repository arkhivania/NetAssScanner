using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Base
{
    public interface INetSearch
    {
        IEnumerable<NamespaceInfo> GetNamespaces();

        IEnumerable<SearchItem> Search(string query, int maxCount);
        void RebuildIndex();
    }
}
