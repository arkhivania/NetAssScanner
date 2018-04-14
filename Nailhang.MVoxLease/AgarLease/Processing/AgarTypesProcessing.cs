using Mono.Cecil;
using Nailhang.IndexBase;
using Nailhang.Processing.ModuleBuilder.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Nailhang.Processing;
using System.Linq;

namespace Nailhang.MVoxLease.AgarLease.Processing
{
    class AgarTypesProcessing : IModuleBuilder
    {
        public IEnumerable<IndexBase.Module> CreateModules(AssemblyDefinition assemblyDefinition)
        {
            foreach (var type in assemblyDefinition.GetTypes())
            {
                if (type.IsNinject())
                    if (!type.FullName.Contains("/"))
                        foreach (var m in ExtractModule(type, assemblyDefinition, null))
                            yield return m;
            }
        }

        private IEnumerable<IndexBase.Module> ExtractModule(TypeDefinition moduleType, AssemblyDefinition assDef, CustomAttribute modAttr)
        {
            var res = new IndexBase.Module();
            res.Assembly = assDef.Name.Name;
            res.Description = moduleType.GetDescription();
            res.FullName = moduleType.FullName + " [Lease]";
            res.Tags = new[] { "Lease" };

            bool isAgar = false;

            if (modAttr != null)
                res.Significance = (Nailhang.Significance)modAttr.Properties.Where(w => w.Name == "Significance").Select(w => (int)w.Argument.Value).FirstOrDefault();
            else
                res.Significance = Significance.Low;

            var types = new List<Mono.Cecil.TypeReference>();
            foreach (var meth in moduleType.Methods
                .Where(w => w.Body != null))
            {
                if (meth.Name == "Load")
                {
                    var methBody = meth.Body;

                    foreach(var i in methBody.Instructions)
                    {                        
                        var op = i.Operand as Mono.Cecil.TypeReference;
                        if (op != null 
                            && op.FullName != "Ninject.Modules.INinjectModule"
                            && op.FullName != "System.Type")
                            types.Add(op);

                        var mr = i.Operand as Mono.Cecil.MethodReference;
                        if(mr != null)
                        {
                            if (mr.DeclaringType.FullName == "Alda.MultiVox.Base.Agar.Base.AgarModule")
                                isAgar = true;
                        }
                    }
                }
            }

            if(isAgar)
            {
                if (types.Any(q => q.FullName == "Alda.MultiVox.Base.Agar.Base.CellInfo"))
                    yield break;

                res.ModuleBinds = types.Select(q => q.ToIndexBaseTypeReference()).ToArray();
                yield return res;
            }
        }
    }
}
