using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodEnvironmentAttribute : Attribute
    {
        public InvocationRestriction InvocationRestriction { get; set; }
    }
}
