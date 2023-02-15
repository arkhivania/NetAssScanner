using Mono.Cecil;
using Mono.Cecil.Cil;
using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Processing.ZoneBuilder.Processing
{
    class ZoneBuilder : Base.IZoneBuilder
    {
        public IEnumerable<Zone> ExtractZones(AssemblyDefinition assemblyDefinition)
        {
            foreach (var module in assemblyDefinition.Modules)
                foreach (var t in module.Types)
                    foreach (var z in ExtractZonesFromType(t))
                        yield return z;
        }

        private IEnumerable<Zone> ExtractZonesFromType(TypeDefinition typeDef)
        {
            foreach (var m in typeDef.Methods.Where(w => w.Body != null))
            {
                var components = new List<string>();
                foreach (var i in m.Body.Instructions)
                {
                    if(i.OpCode == OpCodes.Newobj
                        && i.Operand is MethodDefinition md
                        && md.DeclaringType.BaseType.FullName == "Ninject.Modules.NinjectModule")
                    {
                        components.Add(md.DeclaringType.FullName);
                    }

                    if (i.OpCode == OpCodes.Call &&
                        i.Operand is GenericInstanceMethod gim
                        && gim.Name == "Load"
                        && gim.DeclaringType.FullName == "Ninject.ModuleLoadExtensions")
                    {
                        foreach (var ga in gim.GenericArguments)
                            if (ga.Resolve()?.BaseType?.FullName == "Ninject.Modules.NinjectModule")
                                components.Add(ga.Resolve().FullName);
                    }
                }

                if (components.Any())
                {
                    var zone = new Zone 
                    { 
                        Path = $"{typeDef.FullName}?{m.Name}", 
                        ComponentIds = components.ToArray(), 
                        Description = typeDef.GetDescription()
                    };
                    if (typeDef.BaseType.FullName == "Ninject.Modules.NinjectModule"
                        && m.Name == "Load")
                    {
                        zone.Path = typeDef.FullName;
                        zone.Type = ZoneType.Bootstrap;
                    }

                    yield return zone;
                }
            }

            yield break;
        }
    }
}
