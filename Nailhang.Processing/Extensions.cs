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
                .Where(w => w.AttributeType.FullName == typeof(DescriptionAttribute).FullName)
                .Where(w => w.ConstructorArguments.Count > 0)
                .Select(w => w.ConstructorArguments[0].Value)
                .OfType<string>().FirstOrDefault();
        }

        public static string PrintShortType(this TypeReference type)
        {
            var ginst = type as GenericInstanceType;
            if (ginst != null)
            {
                return string.Format("{0}<{1}>", type.Name,
                    ginst.GenericArguments.Select(w => w.Name).Aggregate((a, b) => a + ", " + b));
            }
            return type.Name;
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

        internal static IEnumerable<TypeDefinition> GetNamespaceTypes(this AssemblyDefinition assemblyDefinition, TypeDefinition moduleType)
        {
            return GetTypes(assemblyDefinition)
                .Where(w => w.Namespace.StartsWith(moduleType.Namespace));
        }

        internal static IEnumerable<TypeReference> GetConstructorTypes(this TypeDefinition type)
        {
            if ((type.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)
                yield break;

            foreach (var c in type.Methods.Where(w => w.IsConstructor))
            {
                foreach (var p in c.Parameters)
                    yield return p.ParameterType;
            }
        }

        internal static IndexBase.ModuleInterface[] CreateInterfaces(this IEnumerable<TypeDefinition> types)
        {
            return types
                    .Where(w =>
                        {
                            return (w.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract &&
                                (w.Attributes & TypeAttributes.Public) == TypeAttributes.Public;
                        })
                        .Select(w => CreateInterface(w)).ToArray();
        }

        internal static IndexBase.ModuleObject[] CreateObjects(this IEnumerable<TypeDefinition> types)
        {
            return types.Where(w =>
                {
                    return (w.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract &&
                        w.BaseType.FullName != typeof(System.Enum).FullName &&
                        (w.Attributes & TypeAttributes.Public) == TypeAttributes.Public;
                })
                .Select(w => CreateModuleObject(w)).ToArray();
        }

        internal static string[] GetDependencies(this IEnumerable<TypeDefinition> types, string homeNamespace)
        {
            return types.SelectMany(w => w.GetConstructorTypes())
                .Select(w => w.Namespace)
                .Where(w => !w.StartsWith(homeNamespace))
                .Where(w => !string.IsNullOrEmpty(w))
                .Distinct()
                .OrderBy(w => w)
                .ToArray();
        }

        private static ModuleObject CreateModuleObject(TypeDefinition objectType)
        {
            var res = new ModuleObject();
            res.Name = objectType.FullName;
            res.Description = objectType.GetDescription();
            return res;
        }

        private static ModuleInterface CreateInterface(TypeDefinition interfaceType)
        {

            var res = new ModuleInterface() { Name = interfaceType.FullName, Description = interfaceType.GetDescription() };
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
