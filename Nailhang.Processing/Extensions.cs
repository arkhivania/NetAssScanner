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

        public static string PrintShortMethod(this MethodDefinition method)
        {
            var mname = method.Name;
            var extra = "";
            var returnType = method.ReturnType.Name;

            if (mname.StartsWith("get_"))
            {
                mname = mname.Substring(4);
                extra = "{ get; }";
            }

            var ginst = method.ReturnType as GenericInstanceType;
            if(ginst != null)
            {
                returnType = string.Format("{0}<{1}>", method.ReturnType.Name,
                    ginst.GenericArguments.Select(w => w.Name).Aggregate((a, b) => a + ", " + b));
            }

            return string.Format("{0} {1}({2}) {3}", returnType, mname, "", extra);
        }
    }
}
