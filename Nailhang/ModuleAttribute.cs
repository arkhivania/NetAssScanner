using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang
{
    [AttributeUsage(AttributeTargets.All)]
    public class ModuleAttribute : Attribute
    {
        public Significance Significance { get; set; }
    }
}
