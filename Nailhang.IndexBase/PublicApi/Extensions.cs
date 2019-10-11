using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public static class Extensions
    {
        public static string GenericsString(this Method method, bool printBrackets = false)
        {
            if (method.GenericParameters.Length == 0)
                return "";

            if (printBrackets)
                return $"<{string.Join(", ", method.GenericParameters)}>";

            return string.Join(", ", method.GenericParameters);
        }

        public static string ParametersString(this Method method)
        {
            if (method.Parameters.Length == 0)
                return "";

            return string.Join(", ", method.Parameters.Select(w => $"{w.Type} {w.Name}"));
        }
    }
}
