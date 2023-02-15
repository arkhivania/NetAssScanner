using Mono.Cecil;
using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Processing.ZoneBuilder.Base
{
    public interface IZoneBuilder
    {
        IEnumerable<Zone> ExtractZones(AssemblyDefinition assemblyDefinition);
    }
}
