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
using Nailhang.Processing.ZoneBuilder.Base;
using Nailhang.IndexBase.Storage;
using Nailhang.IndexBase.Index;

namespace Nailhang.Processing.ModuleBuilder.Processing
{
    class CecilProcessor : IndexBase.Index.IIndexProcessor
    {
        private readonly Base.IModuleBuilder[] moduleBuilders;
        private readonly IEnumerable<IZoneBuilder> zoneBuilders;

        public CecilProcessor(IEnumerable<Base.IModuleBuilder> moduleBuilders,
            IEnumerable<IZoneBuilder> zoneBuilders)
        {
            this.moduleBuilders = moduleBuilders.ToArray();
            this.zoneBuilders = zoneBuilders.ToArray();
        }

        public IEnumerable<ExtractResult> ExtractModules(string filePath)
        {
            var readerParameters = new ReaderParameters();
            var resolver = new DefaultAssemblyResolver();

            var assLocation = typeof(CecilProcessor).GetTypeInfo().Assembly.Location;
            var assDir = Path.GetDirectoryName(assLocation);

            var targetDirFolder = Path.GetDirectoryName(filePath);
            resolver.AddSearchDirectory(targetDirFolder);
            resolver.AddSearchDirectory(assDir);


            foreach (var resolvePath in Environment.GetCommandLineArgs().Where(w => w.ToLower().StartsWith("-cecilrefpath:")))
                resolver.AddSearchDirectory(resolvePath.Substring("-cecilRefPath:".Length));

            readerParameters.AssemblyResolver = resolver;

            if (new FileInfo(filePath).Exists == false)
            {
                if (new FileInfo(Path.Combine(assDir, filePath)).Exists)
                    filePath = Path.Combine(assDir, filePath);
            }

            var assDef = AssemblyDefinition.ReadAssembly(filePath, readerParameters);

            foreach (var mb in moduleBuilders)
                foreach (var module in mb.CreateModules(assDef))
                    yield return new ExtractResult { Module = module };

            foreach (var mb in zoneBuilders)
                foreach (var zone in mb.ExtractZones(assDef))
                    yield return new ExtractResult { Zone = zone };
        }


    }
}
