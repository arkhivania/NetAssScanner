using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang
{
    [AttributeUsage(AttributeTargets.All)]
    public class ModuleDescription : Attribute
    {
        public string Description { get; set; }

        public ModuleDescription(string description)
        {
            this.Description = description;
        }
    }
}
