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
            var ninjectModule = typeof(Ninject.Modules.NinjectModule).FullName;


            foreach (var type in assemblyDefinition.GetTypes())
            {
                var modAttr = type.CustomAttributes.FirstOrDefault(w => w.AttributeType.FullName == moduleType);
                if (modAttr != null)
                    yield return ExtractModule(type, assemblyDefinition, modAttr);
                else if(type.BaseType != null && type.BaseType.FullName == ninjectModule)
                    if(!type.FullName.Contains("/"))
                        yield return ExtractModule(type, assemblyDefinition, null);
            }
        }

        private Module ExtractModule(TypeDefinition moduleType, AssemblyDefinition assDef, CustomAttribute modAttr)
        {
            var res = new Module();
            res.Assembly = assDef.Name.Name;
            res.Description = moduleType.GetDescription();
            res.FullName = moduleType.FullName;
            res.Namespace = moduleType.Namespace;
            if (modAttr != null)
            {
                res.Significance = (Nailhang.Significance)modAttr.Properties.Where(w => w.Name == "Significance").Select(w => (int)w.Argument.Value).FirstOrDefault();
            }
            else
                res.Significance = Significance.Low;

            var moduleTypes = assDef.GetNamespaceTypes(moduleType).ToArray();

            res.Interfaces = moduleTypes.CreateInterfaces();
            res.Objects = moduleTypes.CreateObjects();
            res.NamespaceDependencies = moduleTypes.GetDependencies(moduleType.Namespace);

            var bindsList = new HashSet<IndexBase.TypeReference>();
            foreach(var meth in moduleType.Methods)
            {
                if(meth.Name == "Load")
                {
                    var methBody = meth.Body;
                    foreach(var i in methBody.Instructions)
                    {
                        var genInstance = i.Operand as GenericInstanceMethod;
                        if(genInstance != null)
                            if(genInstance.ElementMethod.Name == "Bind")
                            {
                                foreach(var bt in genInstance.GenericArguments)
                                    bindsList.Add(bt.ToIndexBaseTypeReference());
                            }
                    }
                }
            }

            res.ModuleBinds = bindsList.ToArray();

            return res;
        }
    }
}
