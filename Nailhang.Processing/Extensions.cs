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
    }
}
