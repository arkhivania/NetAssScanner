using Mono.Cecil;
using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Processing.ZoneBuilder.Processing
{
    class ZoneBuilder : Base.IZoneBuilder
    {
        public IEnumerable<Zone> ExtractZones(AssemblyDefinition assemblyDefinition)
        {
            yield break;
        }
    }
}
