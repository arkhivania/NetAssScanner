using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Nailhang.IndexBase.PublicApi;
using Nailhang.Processing.ModuleBuilder.Processing;

namespace Nailhang.Processing.PublicExtract.Processing
{
    class CecilExtract : IPublicProcessor
    {
        private readonly Base.IClassExtractor[] extractors;

        public CecilExtract(Base.IClassExtractor[] extractors)
        {
            this.extractors = extractors;
        }

        public IEnumerable<AssemblyPublic> Extract(string filePath)
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
            var res = new AssemblyPublic() { AssemblyVersion = assDef.Name.Version, FullName = assDef.FullName };

            res.Classes = assDef.Modules.SelectMany(module => extractors.SelectMany(e => e.ExtractClasses(module)))
                .ToArray();

            yield return res;
        }
    }
}
