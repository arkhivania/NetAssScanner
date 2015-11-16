using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.IndexBase
{
    public struct InterfaceMethod
    {
        public string Description { get; set; }
        public string MethodString { get; set; }
        public Nailhang.InvocationRestriction? InvocationRestriction { get; set; }
    }
}
