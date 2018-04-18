using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.History.Base
{
    public interface IHistoryStorage
    {
        void StoreChangeToNamespace(string @namespace, Change change);
        IEnumerable<Change> GetChanges(string @namespace);
        void DropHistory();
    }
}
