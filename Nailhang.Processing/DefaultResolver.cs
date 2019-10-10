using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Processing
{
    class DefaultAssemblyResolver : IAssemblyResolver
    {
        readonly IEnumerable<string> windowsGACFolders;

        readonly HashSet<string> folders = new HashSet<string>() { @".\" };

        readonly Dictionary<string, AssemblyDefinition> definitions = new Dictionary<string, AssemblyDefinition>();

        public DefaultAssemblyResolver()
        {
            var windowsFolder = Environment.GetEnvironmentVariable("windir");
            if (windowsFolder != null)
            {
                windowsGACFolders = new[]
                        {
                        @"assembly\GAC_MSIL",
                        @"assembly\GAC_64",
                        @"assembly\GAC_32",
                        @"Microsoft.NET\assembly\GAC_MSIL",
                        @"Microsoft.NET\assembly\GAC_64",
                        @"Microsoft.NET\assembly\GAC_32",
                    }.Select(q => Path.Combine(windowsFolder, q))
                        .ToArray();
            }
            else
                windowsGACFolders = new string[] { };

        }

        public void AddSearchDirectory(string directory)
        {
            folders
                .UnionWith(new[] { directory });
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
                if (new FileInfo(Path.Combine(f, name)).Exists)
                    return Path.Combine(f, name);

            return null;
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            lock (definitions)
            {
                if (definitions.ContainsKey(name.FullName))
                    return definitions[name.FullName];

                var def = _Resolve(name, parameters);
                definitions[name.FullName] = def;
                return def;
            }
        }

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            throw new NotImplementedException();
        }

        IEnumerable<string> GACSearch(string gacFolder, AssemblyNameReference name)
        {
            gacFolder = Path.Combine(gacFolder, name.Name);
            if (Directory.Exists(gacFolder))
            {
                foreach (var dir_name in new[]
                {
                    name.Version.ToString() + "__" + string.Concat(name.PublicKeyToken.Select(i => i.ToString("x2"))),
                    "v4.0_" + name.Version.ToString() + "__" + string.Concat(name.PublicKeyToken.Select(i => i.ToString("x2")))
                })
                {
                    var assFolder = Path.Combine(gacFolder, dir_name);
                    if (Directory.Exists(assFolder))
                    {
                        var assFilePath = Path.Combine(assFolder, name.Name + ".dll");
                        if (File.Exists(assFilePath))
                            yield return assFilePath;
                    }
                }
            }
        }

        AssemblyDefinition _Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            var filePath = SearchFile(name.Name + ".dll");
            if (filePath == null)
            {
                if (windowsGACFolders.Any())
                {
                    var gacFile = windowsGACFolders.SelectMany(q => GACSearch(q, name)).FirstOrDefault();
                    if (gacFile != null)
                        return AssemblyDefinition.ReadAssembly(gacFile, parameters);

                    throw new InvalidOperationException($"Can't find: {name.FullName} assembly");
                }
            }
            return AssemblyDefinition.ReadAssembly(filePath, parameters);
        }

        public void Dispose()
        {
            definitions.Clear();
        }
    }
}
