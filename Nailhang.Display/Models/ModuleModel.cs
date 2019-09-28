using Nailhang.IndexBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nailhang.Display.Models
{
    public class ModuleModel
    {
        public Module Module { get; set; }

        public string Namespace
        {
            get
            {
                return Module.FullName.ToNamespace();
            }
        }
    }
}