using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Base
{
    public interface INetSearch
    {
        IEnumerable<SearchItem> Search(string query, int maxCount);
    }
}
