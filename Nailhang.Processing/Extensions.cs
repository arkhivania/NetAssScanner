using Mono.Cecil;
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
    }
}
