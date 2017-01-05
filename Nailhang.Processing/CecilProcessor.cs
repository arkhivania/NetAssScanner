using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailhang.IndexBase;
using Mono.Cecil;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace Nailhang.Processing
{
    class CecilProcessor : IndexBase.Index.IIndexProcessor
    {
        private readonly ModuleBuilder.Base.IModuleBuilder[] moduleBuilders;

        public CecilProcessor(IEnumerable<ModuleBuilder.Base.IModuleBuilder> moduleBuilders)
        {
            this.moduleBuilders = moduleBuilders.ToArray();
        }

        public IEnumerable<IndexBase.Module> ExtractModules(string filePath)
        {
            var readerParameters = new ReaderParameters();
            var resolver = new DefaultAssemblyResolver();

            var assLocation = typeof(CecilProcessor).GetTypeInfo().Assembly.Location;
            var assDir = System.IO.Path.GetDirectoryName(assLocation);

            var targetDirFolder = Path.GetDirectoryName(filePath);
            resolver.AddSearchDirectory(targetDirFolder);
            resolver.AddSearchDirectory(assDir);
            

            foreach (var resolvePath in Environment.GetCommandLineArgs().Where(w => w.ToLower().StartsWith("-cecilrefpath:")))
                resolver.AddSearchDirectory(resolvePath.Substring("-cecilRefPath:".Length));

            readerParameters.AssemblyResolver = resolver;

            if(new FileInfo(filePath).Exists == false)
            {
                if (new FileInfo(Path.Combine(assDir, filePath)).Exists)
                    filePath = Path.Combine(assDir, filePath);
            }

            var assDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(filePath, readerParameters);

            foreach (var mb in moduleBuilders)
                foreach(var module in mb.CreateModules(assDef))
                    yield return module;
        }
 
        
    }
}
