using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Processing.ModuleBuilder
{
    class NamespaceGroupedBuilder : Base.IModuleBuilder
    {
        public IEnumerable<IndexBase.Module> CreateModules(Mono.Cecil.AssemblyDefinition assemblyDefinition)
        {
            var describedTypes = (from t in assemblyDefinition.GetTypes()
                                  where t.GetDescription() != null
                                  group t by t.Namespace).ToArray();

            var nhmodules = new NailhangModuleBuilder().CreateModules(assemblyDefinition).ToArray();

            foreach(var g in describedTypes)
            {
                var module = CreateModule(assemblyDefinition, g.ToArray(), nhmodules);
                if (module != null)
                    yield return module;
            }
        }

        private IndexBase.Module CreateModule(AssemblyDefinition assemblyDefinition, Mono.Cecil.TypeDefinition[] typeDefinition, IndexBase.Module[] nhmodules)
        {
            var res = new IndexBase.Module();
            var ftype = typeDefinition.First();

            res.FullName = ftype.Namespace;
            res.Assembly = assemblyDefinition.Name.Name;
            res.Namespace = ftype.Namespace;

            var moduleTypes = typeDefinition
                .Where(w => !nhmodules.Any(w2 => ModuleContains(w2, w))).ToArray();

            if (moduleTypes.Length == 0)
                return null;

            res.Objects = moduleTypes
                .Select(w => new IndexBase.ModuleObject { Name = w.FullName, Description = w.GetDescription() })
                .ToArray();

            res.NamespaceDependencies = moduleTypes.GetDependencies(typeDefinition.First().Namespace);
            res.Interfaces = moduleTypes.CreateInterfaces();
            res.Objects = moduleTypes.CreateObjects();
            res.Description = CreateDescription(moduleTypes.Where(w => !string.IsNullOrEmpty(w.GetDescription())));

            return res;
        }

        private string CreateDescription(IEnumerable<TypeDefinition> types)
        {
            var sb = new StringBuilder();
            foreach(var t in types)
                sb.AppendLine(string.Format("{0}\r\n{1}", t.FullName, t.GetDescription()));
            return sb.ToString();
        }

        private bool ModuleContains(IndexBase.Module module, Mono.Cecil.TypeDefinition type)
        {
            if (module.Namespace == type.Namespace)
                return true;
            return module.Objects.Any(w => w.Name == type.FullName);
        }
    }
}
