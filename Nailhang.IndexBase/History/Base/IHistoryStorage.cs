using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.History.Base
{
    public interface IHistoryStorage
    {
        int LastRevision { get; }
        void StoreChangeToNamespace(string @namespace, Revision revision);
        IEnumerable<Revision> GetChanges(string @namespace);
    }
}
