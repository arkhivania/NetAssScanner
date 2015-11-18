using Mono.Cecil;
using Nailhang.IndexBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Processing
{
    public static class Extensions
    {
        public static string GetDescription(this TypeDefinition typeDefinition)
        {
            return typeDefinition
                .CustomAttributes
                .Where(w => w.AttributeType.FullName == typeof(DescriptionAttribute).FullName || w.AttributeType.FullName == typeof(Nailhang.ModuleDescription).FullName)
                .Where(w => w.ConstructorArguments.Count > 0)
                .Select(w => w.ConstructorArguments[0].Value)
                .OfType<string>().FirstOrDefault();
        }

        public static string PrintShortType(this Mono.Cecil.TypeReference type)
        {
            var ginst = type as GenericInstanceType;
            if (ginst != null)
            {
                return string.Format("{0}<{1}>", type.Name,
                    ginst.GenericArguments.Select(w => w.Name).Aggregate((a, b) => a + ", " + b));
            }
            return type.Name;
        }

        public static IndexBase.TypeReference ToIndexBaseTypeReference(this Mono.Cecil.TypeReference typeReference)
        {
            return new IndexBase.TypeReference
            {
                AssemblyName = typeReference.Module.Assembly.Name.Name,
                FullName = typeReference.FullName
            };
        }

        public static string PrintParameters(this IEnumerable<ParameterDefinition> parameters)
        {
            if (parameters.Any())
            {
                var smallPrint = new Func<ParameterDefinition, string>(p => 
                {
                    var pt = p.ParameterType.PrintShortType();
                    return string.Format("{0} {1}", pt, p.Name);
                });
                return parameters
                    .Select(w => smallPrint(w)).Aggregate((a, b) => a + ", " + b);
            }
            return "";
        }

        public static string PrintShortMethod(this MethodDefinition method)
        {
            var mname = method.Name;
            var extra = "";
            

            if (mname.StartsWith("get_"))
            {
                mname = mname.Substring(4);
                extra = "{ get; }";
            }

            return string.Format("{0} {1}({2}) {3}", method.ReturnType.PrintShortType(), 
                mname, 
                method.Parameters.PrintParameters(), extra);
        }

        internal static IEnumerable<TypeDefinition> GetTypes(this AssemblyDefinition assemblyDefinition)
        {
            return assemblyDefinition.MainModule.GetTypes();
        }

        //internal static IEnumerable<TypeDefinition> GetNamespaceTypes(this AssemblyDefinition assemblyDefinition, string nameSpace)
        //{
        //    return GetTypes(assemblyDefinition)
        //        .Where(w => w.Namespace.StartsWith(nameSpace))
        //        .Where(w => w.FullName.Substring(nameSpace.Length).StartsWith(".") || w.Namespace.Equals(nameSpace));
        //}

        internal static IEnumerable<Mono.Cecil.TypeReference> GetConstructorTypes(this TypeDefinition type)
        {
            if ((type.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)
                yield break;

            foreach (var c in type.Methods.Where(w => w.IsConstructor))
            {
                foreach (var p in c.Parameters)
                    if(p.ParameterType != null)
                        yield return p.ParameterType;
            }
        }

        public static IEnumerable<TypeDefinition> FilterInterfaces(this IEnumerable<TypeDefinition> types)
        {
            return types
                    .Where(w =>
                    {
                        return (w.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract &&
                            (w.Attributes & TypeAttributes.Public) == TypeAttributes.Public;
                    });
        }

        internal static IndexBase.ModuleInterface[] CreateInterfaces(this IEnumerable<TypeDefinition> types)
        {
            return types
                    .FilterInterfaces()
                    .Select(w => CreateInterface(w)).ToArray();
        }

        public static TypeDefinition ToDef(this Mono.Cecil.TypeReference tr)
        {
            var res = tr.Module.MetadataResolver.Resolve(tr);
            if (res == null)
                throw new InvalidOperationException();
            return res;
        }

        public static IEnumerable<TypeDefinition> ToDefs(this IEnumerable<Mono.Cecil.TypeReference> refs)
        {
            return refs
                .ToDefsRaw()
                .Distinct();
        }

        static IEnumerable<TypeDefinition> ToDefsRaw(this IEnumerable<Mono.Cecil.TypeReference> refs)
        {
            foreach (var r in refs)
                yield return r.ToDef();
        }

        public static IEnumerable<TypeDefinition> FilterObjects(this IEnumerable<TypeDefinition> types, bool onlyPublic)
        {
            return types
                .Where(w =>
                {
                    return (w.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract 
                    && w.BaseType != null
                    && w.BaseType.FullName != typeof(System.Enum).FullName;
                }).Where(w => onlyPublic ? (w.Attributes & TypeAttributes.Public) == TypeAttributes.Public : true);
        }

        internal static IndexBase.ModuleObject[] CreateObjects(this IEnumerable<TypeDefinition> types, bool onlyPublic)
        {
            return types
                .FilterObjects(onlyPublic)
                .Select(w => CreateModuleObject(w)).ToArray();
        }

        public static IDictionary<Mono.Cecil.TypeReference, Mono.Cecil.TypeDefinition> LoadDefinitions(this IEnumerable<Mono.Cecil.TypeReference> types, AssemblyDefinition assDef)
        {
            var dict = new Dictionary<Mono.Cecil.TypeReference, Mono.Cecil.TypeDefinition>();
            var modTypes = assDef.Modules.SelectMany(w => w.Types).ToDictionary(w => w.FullName, w => w);

            foreach (var m in types)
                if (modTypes.ContainsKey(m.FullName))
                    dict[m] = modTypes[m.FullName];

            return dict;
        }

        internal static IEnumerable<Mono.Cecil.TypeReference> GetDependencies(this IEnumerable<TypeDefinition> types)
        {
            return types.SelectMany(w => w.GetConstructorTypes())
                .Select(w => w)
                .Distinct();
        }

        private static ModuleObject CreateModuleObject(TypeDefinition objectType)
        {
            var res = new ModuleObject();
            res.TypeReference = objectType.ToIndexBaseTypeReference();
            res.Description = objectType.GetDescription();
            return res;
        }

        private static ModuleInterface CreateInterface(TypeDefinition interfaceType)
        {
            var res = new ModuleInterface()
            {
                TypeReference = interfaceType.ToIndexBaseTypeReference(),
                Description = interfaceType.GetDescription()
            };
            res.Methods = interfaceType.Methods
                .Select(meth => CreateMethod(meth)).ToArray();

            return res;
        }

        private static Nailhang.IndexBase.InterfaceMethod CreateMethod(MethodDefinition meth)
        {
            return new InterfaceMethod() { MethodString = meth.PrintShortMethod(), InvocationRestriction = GetRestriction(meth) };
        }

        private static InvocationRestriction? GetRestriction(MethodDefinition meth)
        {
            var meaname = typeof(Nailhang.MethodEnvironmentAttribute).FullName;
            var mea = meth.CustomAttributes.Where(w => w.AttributeType.FullName == meaname).FirstOrDefault();
            if (mea != null)
                return (Nailhang.InvocationRestriction)(int)mea.Properties.First(w => w.Name == "InvocationRestriction").Argument.Value;
            return null;
        }
    }
}
