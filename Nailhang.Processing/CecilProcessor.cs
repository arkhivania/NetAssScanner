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

        IEnumerable<TypeDefinition> GetNamespaceTypes(AssemblyDefinition assemblyDefinition, TypeDefinition moduleType)
        {
            return GetTypes(assemblyDefinition)
                .Where(w => w.Namespace.StartsWith(moduleType.Namespace));
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
            res.Interfaces = GetNamespaceTypes(assDef, moduleType)
                .Where(w => 
                {
                    return (w.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract &&
                        (w.Attributes & TypeAttributes.Public) == TypeAttributes.Public;
                })
                .Select(w => CreateInterface(w)).ToArray();

            res.Objects = GetNamespaceTypes(assDef, moduleType)
                .Where(w =>
                {
                    return (w.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract &&
                        w.BaseType.FullName != typeof(System.Enum).FullName &&
                        (w.Attributes & TypeAttributes.Public) == TypeAttributes.Public;
                })
                .Select(w => CreateModuleObject(w)).ToArray();

            return res;
        }

        private ModuleObject CreateModuleObject(TypeDefinition objectType)
        {
            var res = new ModuleObject();
            res.Name = objectType.FullName;
            res.Description = objectType.GetDescription();
            return res;
        }

        private ModuleInterface CreateInterface(TypeDefinition interfaceType)
        {
            
            var res = new ModuleInterface() { Name = interfaceType.FullName, Description = interfaceType.GetDescription() };
            res.Methods = interfaceType.Methods
                .Select(meth => CreateMethod(meth)).ToArray();

            return res;
        }

        private Nailhang.IndexBase.InterfaceMethod CreateMethod(MethodDefinition meth)
        {
            return new InterfaceMethod() { MethodString = meth.PrintShortMethod() };
        }
    }
}
