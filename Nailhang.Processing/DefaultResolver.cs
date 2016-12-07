using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Processing
{
    class DefaultAssemblyResolver : IAssemblyResolver
    {
        readonly HashSet<string> folders = new HashSet<string>() { @".\" };

        public void AddSearchDirectory(string directory)
        {
            folders.UnionWith(new[] { directory });
        }

        public AssemblyDefinition Resolve(string fullName)
        {
            throw new NotImplementedException();
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return Resolve(name, new ReaderParameters { AssemblyResolver = this });
        }

        string SearchFile(string name)
        {
            foreach (var f in folders)
                if (new FileInfo(System.IO.Path.Combine(f, name)).Exists)
                    return System.IO.Path.Combine(f, name);

            return null;
        }

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {   
            throw new NotImplementedException();
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            var filePath = SearchFile(name.Name + ".dll");
            if (filePath == null)
                throw new InvalidOperationException($"Can't find: {name.FullName} assembly");
            return AssemblyDefinition.ReadAssembly(filePath, parameters);
        }

        public void Dispose()
        {

        }
    }
}
