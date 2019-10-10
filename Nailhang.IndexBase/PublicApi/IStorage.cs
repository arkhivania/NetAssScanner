using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public interface IStorage
    {
        void UpdateAssemblies(IEnumerable<AssemblyPublic> assemblies);
        IEnumerable<AssemblyPublic> LoadAssemblies();
    }
}
