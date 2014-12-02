using Mono.Cecil;
using Nailhang.IndexBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Processing.ModuleBuilder
{
    class NailhangModuleBuilder : Base.IModuleBuilder
    {
        public IEnumerable<Module> CreateModules(Mono.Cecil.AssemblyDefinition assemblyDefinition)
        {
            var moduleType = typeof(Nailhang.ModuleAttribute).FullName;

            foreach (var type in assemblyDefinition.GetTypes())
            {
                var modAttr = type.CustomAttributes.FirstOrDefault(w => w.AttributeType.FullName == moduleType);
                if (modAttr != null)
                {
                    yield return ExtractModule(type, assemblyDefinition, modAttr);
                }
            }
        }

        private Module ExtractModule(TypeDefinition moduleType, AssemblyDefinition assDef, CustomAttribute modAttr)
        {
            var res = new Module();
            res.Assembly = assDef.Name.Name;
            res.Description = moduleType.GetDescription();
            res.FullName = moduleType.FullName;
            res.Namespace = moduleType.Namespace;
            res.Significance = (Nailhang.Significance)modAttr.Properties.Where(w => w.Name == "Significance").Select(w => (int)w.Argument.Value).FirstOrDefault();

            var moduleTypes = assDef.GetNamespaceTypes(moduleType).ToArray();

            res.Interfaces = moduleTypes.CreateInterfaces();
            res.Objects = moduleTypes.CreateObjects();
            res.NamespaceDependencies = moduleTypes.GetDependencies(moduleType.Namespace);

            return res;
        }

        

        

        
    }
}
