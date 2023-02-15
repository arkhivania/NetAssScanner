using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.Storage
{
    public interface IZonesStorage
    {
        void StoreZone(Zone zone);
        IEnumerable<Zone> GetZones();
        long DropZones(string namespaceStartsWith);
    }
}
