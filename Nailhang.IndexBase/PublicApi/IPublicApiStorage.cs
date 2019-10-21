using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public interface IPublicApiStorage
    {
        void UpdateAssemblies(IEnumerable<(AssemblyPublic, Class[])> assemblies);
        IEnumerable<AssemblyPublic> LoadAssemblies();
        IEnumerable<Class> LoadClasses(string assemblyId);
        AssemblyPublic? LoadAssembly(string fullName);
        IEnumerable<AssemblyPublic> FindByShortName(string shortName);

        long Drop(DropRequest dropRequest);
    }
}
