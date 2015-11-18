using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.IndexBase
{
    public struct TypeReference
    {
        public string AssemblyName { get; set; }
        public string FullName { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}
