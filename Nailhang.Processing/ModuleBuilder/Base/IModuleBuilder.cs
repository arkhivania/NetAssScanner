using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Processing.ModuleBuilder.Base
{
    public interface IModuleBuilder
    {
        IEnumerable<IndexBase.Module> CreateModules(AssemblyDefinition assemblyDefinition);
    }
}
