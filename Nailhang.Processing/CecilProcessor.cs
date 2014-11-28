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

            var readerParameters = new ReaderParameters();
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(System.IO.Path.GetDirectoryName(typeof(CecilProcessor).Assembly.Location));

            foreach (var resolvePath in Environment.GetCommandLineArgs().Where(w => w.ToLower().StartsWith("-cecilrefpath:")))
                resolver.AddSearchDirectory(resolvePath.Substring("-cecilRefPath:".Length));

            readerParameters.AssemblyResolver = resolver;

            var assDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(filePath, readerParameters);
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

            res.NamespaceDependencies = GetNamespaceTypes(assDef, moduleType)
                .SelectMany(w => GetConstructorTypes(w))
                .Concat(GetNamespaceTypes(assDef, moduleType).SelectMany(w => GetMethodTypes(w)))
                .Select(w => w.Namespace)
                .Where(w => !w.StartsWith(moduleType.Namespace))
                .Distinct()
                .OrderBy(w => w)
                .ToArray();

            return res;
        }

        private IEnumerable<TypeReference> GetMethodTypes(TypeDefinition w)
        {
            return Enumerable.Empty<TypeReference>();
        }

        private IEnumerable<TypeReference> GetConstructorTypes(TypeDefinition type)
        {
            if ((type.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)
                yield break;

            foreach(var c in type.Methods.Where(w => w.IsConstructor))
            {
                foreach (var p in c.Parameters)
                    yield return p.ParameterType;
            }
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
            return new InterfaceMethod() { MethodString = meth.PrintShortMethod(), InvocationRestriction = GetRestriction(meth) };
        }

        private InvocationRestriction? GetRestriction(MethodDefinition meth)
        {
            var meaname = typeof(Nailhang.MethodEnvironmentAttribute).FullName;
            var mea = meth.CustomAttributes.Where(w => w.AttributeType.FullName == meaname).FirstOrDefault();
            if(mea != null)
                return (Nailhang.InvocationRestriction)(int)mea.Properties.First(w => w.Name == "InvocationRestriction").Argument.Value;
            return null;
        }
    }
}
