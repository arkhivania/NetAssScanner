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
        IEnumerable<TypeDefinition> GetTypes(AssemblyDefinition assemblyDefinition)
        {
            return assemblyDefinition.MainModule.GetTypes();
        }

        public IEnumerable<IndexBase.Module> ExtractModules(string filePath)
        {
            var moduleType = typeof(Nailhang.ModuleAttribute).FullName;

            var assDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(filePath);
            foreach(var type in GetTypes(assDef))
            {
                var modAttr = type.CustomAttributes.FirstOrDefault(w => w.AttributeType.FullName == moduleType);
                if(modAttr != null)
                {
                    yield return ExtractModule(type, assDef, modAttr);
                }
            }
        }
 
        private Module ExtractModule(TypeDefinition moduleType, AssemblyDefinition assDef, CustomAttribute modAttr)
        {
            var res = new Module();
            res.Assembly = assDef.Name.Name;
            res.Description = moduleType.GetDescription();
            res.FullName = moduleType.FullName;
            res.Significance = (Nailhang.Significance)(int)modAttr.Properties.FirstOrDefault(w => w.Name == "Significance").Argument.Value;
            res.Interfaces = GetTypes(assDef)
                .Where(w => w.Namespace.StartsWith(moduleType.Namespace))
                .Where(w => 
                {
                    return (w.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract &&
                        (w.Attributes & TypeAttributes.Public) == TypeAttributes.Public;
                })
                .Select(w => CreateInterface(w, assDef)).ToArray();
            return res;
        }

        private ModuleInterface CreateInterface(TypeDefinition interfaceType, AssemblyDefinition assDef)
        {
            var res = new ModuleInterface() { Name = interfaceType.FullName, Description = interfaceType.GetDescription() };
            return res;
        }
    }
}
