using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailhang.IndexBase;
using Mono.Cecil;
using System.ComponentModel;

namespace Nailhang.Processing
{
    class CecilProcessor : IndexBase.Index.IIndexProcessor
    {
        private readonly IEnumerable<ModuleBuilder.Base.IModuleBuilder> moduleBuilders;

        public CecilProcessor(IEnumerable<ModuleBuilder.Base.IModuleBuilder> moduleBuilders)
        {
            this.moduleBuilders = moduleBuilders;
        }

        public IEnumerable<IndexBase.Module> ExtractModules(string filePath)
        {
            var readerParameters = new ReaderParameters();
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(System.IO.Path.GetDirectoryName(typeof(CecilProcessor).Assembly.Location));

            foreach (var resolvePath in Environment.GetCommandLineArgs().Where(w => w.ToLower().StartsWith("-cecilrefpath:")))
                resolver.AddSearchDirectory(resolvePath.Substring("-cecilRefPath:".Length));

            readerParameters.AssemblyResolver = resolver;

            var assDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(filePath, readerParameters);

            foreach (var mb in moduleBuilders)
                foreach(var module in mb.CreateModules(assDef))
                    yield return module;
        }
 
        
    }
}
