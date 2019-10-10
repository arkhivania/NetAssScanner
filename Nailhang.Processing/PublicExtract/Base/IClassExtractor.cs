using Mono.Cecil;
using Nailhang.IndexBase.PublicApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Processing.PublicExtract.Base
{
    public interface IClassExtractor
    {
        IEnumerable<Class> ExtractClasses(ModuleDefinition module);
    }
}
