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
                    yield return ExtractModule(type, assemblyDefinition, modAttr);
                else
                {
                    if(IsNinject(type))
                        if (!type.FullName.Contains("/"))
                            yield return ExtractModule(type, assemblyDefinition, null);
                }
            }
        }

        bool IsNinject(TypeDefinition type)
        {
            try
            {
                var interfaces = GetBases(type)
                    .ToDefs()
                    .SelectMany(w => w.Interfaces)
                    .ToDefs();
                return interfaces.Any(w => w.FullName == typeof(Ninject.Modules.INinjectModule).FullName);
            }
            catch
            {
                return false;
            }
        }

        IEnumerable<Mono.Cecil.TypeReference> GetBases(Mono.Cecil.TypeDefinition t)
        {
            if (t.BaseType == null)
                yield break;

            foreach (var b in GetBases(t.BaseType.ToDef()))
                yield return b;

            yield return t;
        }

        private bool IsInstanceOf(TypeDefinition type, string baseFullName)
        {
            if (type.BaseType == null)
                return false;
            if (GetBases(type).Any(w => w.FullName == baseFullName))
                return true;

            

            return false;
        }

        static IEnumerable<Mono.Cecil.TypeReference> ModuleActivates(Mono.Cecil.TypeDefinition module)
        {
            foreach (var meth in module.Methods
                .Where(w => w.Body != null))
            {
                var methBody = meth.Body;
                foreach (var i in methBody.Instructions)
                {
                    var genInstance = i.Operand as GenericInstanceMethod;
                    if (genInstance != null)
                    {
                        if (genInstance.ElementMethod.Name == "To")
                        {
                            foreach (var bt in genInstance.GenericArguments)
                                yield return bt;
                        }

                        if(genInstance.ElementMethod.Name == "Get")
                        {
                            foreach (var bt in genInstance.GenericArguments)
                                yield return bt;
                        }
                    }
                }
            }
        }
        

        static IEnumerable<Mono.Cecil.TypeReference> ModuleBinds(Mono.Cecil.TypeDefinition module)
        {
            foreach (var meth in module.Methods
                .Where(w => w.HasBody))
            {
                if (meth.Name == "Load")
                {
                    var methBody = meth.Body;
                    foreach (var i in methBody.Instructions)
                    {
                        var genInstance = i.Operand as GenericInstanceMethod;
                        if (genInstance != null)
                            if (genInstance.ElementMethod.Name == "Bind")
                            {
                                foreach (var bt in genInstance.GenericArguments)
                                    yield return bt;
                            }
                    }
                }
            }
        }

        private Module ExtractModule(TypeDefinition moduleType, AssemblyDefinition assDef, CustomAttribute modAttr)
        {
            var res = new Module();
            res.Assembly = assDef.Name.Name;
            res.Description = moduleType.GetDescription();
            res.FullName = moduleType.FullName;

            if (modAttr != null)
                res.Significance = (Nailhang.Significance)modAttr.Properties.Where(w => w.Name == "Significance").Select(w => (int)w.Argument.Value).FirstOrDefault();
            else
                res.Significance = Significance.Low;

            var moduleTypeRefs = ModuleBinds(moduleType).ToArray();

            var activatesTypes = ModuleActivates(moduleType)
                .ToDefs()
                .ToArray();

            activatesTypes = activatesTypes
                .Concat(moduleTypeRefs.ToDefs())
                .Concat(activatesTypes.SelectMany(w => w.GetConstructorTypes()).ToDefs())
                .Distinct()
                .ToArray();

            var moduleTypes = activatesTypes;

            res.ModuleBinds = ModuleBinds(moduleType)
                .Select(w => w.ToIndexBaseTypeReference())
                .ToArray();

            res.Interfaces = moduleTypes
                .Where(w => w.Namespace.StartsWith(moduleType.Namespace))
                .CreateInterfaces();
            res.Objects = moduleTypes
                .Where(w => w.Namespace.StartsWith(moduleType.Namespace))
                .CreateObjects(true);

            var moduleReferences = moduleTypes
                .FilterObjects(false)
                .GetDependencies()
                .ToDefs();

            res.InterfaceDependencies = moduleReferences
                .FilterInterfaces()
                .Select(w => w.ToIndexBaseTypeReference())
                .ToArray();

            res.ObjectDependencies = moduleReferences
                .FilterObjects(true)
                .Select(w => w.ToIndexBaseTypeReference())
                .ToArray();

            return res;
        }
    }
}
