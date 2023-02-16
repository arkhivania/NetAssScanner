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
            var extracted = new List<Zone>();
            var privateExtracted = new HashSet<TypeDefinition>();


            foreach (var module in assemblyDefinition.Modules)
                foreach (var t in module.Types)
                {
                    try
                    {
                        foreach (var z in ExtractZonesFromType(t))
                            extracted.Add(z);
                    }
                    catch
                    {

                    }

                    try
                    {
                        foreach (var et in PrivateTypes(t))
                            if (privateExtracted.Add(et))
                            {
                                foreach (var z in ExtractZonesFromType(et))
                                    extracted.Add(z);
                            }
                    }
                    catch
                    {

                    }
                }

            return extracted;
        }

        private IEnumerable<TypeDefinition> PrivateTypes(TypeDefinition t)
        {
            var hs = new HashSet<TypeDefinition>();
            foreach (var m in t.Methods)
                if (m.Body != null)
                    foreach (var i in m.Body.Instructions)
                    {
                        if (i.OpCode == OpCodes.Newobj
                            && i.Operand is MethodDefinition md
                            && (md.DeclaringType.Attributes & TypeAttributes.NestedPrivate) != 0)
                            if (hs.Add(md.DeclaringType))
                                yield return md.DeclaringType;
                    }
        }

        private IEnumerable<Zone> ExtractZonesFromType(TypeDefinition typeDef)
        {
            foreach (var m in typeDef.Methods.Where(w => w.Body != null))
            {
                var components = new List<string>();
                foreach (var i in m.Body.Instructions)
                {
                    if (i.OpCode == OpCodes.Newobj
                        && i.Operand is MethodReference mr)
                    {
                        var br = mr.DeclaringType.Resolve();
                        if (br != null && br.BaseType?.FullName == "Ninject.Modules.NinjectModule")
                            components.Add(br.FullName);
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

                    if (zone.Path.EndsWith("?MoveNext"))
                        zone.Path = zone.Path.Substring(0, zone.Path.Length - "?MoveNext".Length);

                    if (typeDef.CustomAttributes.Any(q => q.AttributeType.FullName == "NUnit.Framework.TestFixtureAttribute"))
                        zone.Type = ZoneType.Test;

                    yield return zone;
                }
            }

            yield break;
        }
    }
}
