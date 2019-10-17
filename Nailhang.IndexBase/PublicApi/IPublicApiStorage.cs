using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public interface IPublicApiStorage
    {
        void UpdateAssemblies(IEnumerable<AssemblyPublic> assemblies);
        IEnumerable<AssemblyPublic> LoadAssemblies();
        IEnumerable<AssemblyPublic> LoadAssembly(string fullName);

        long Drop(DropRequest dropRequest);
    }
}
