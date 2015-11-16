using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.IndexBase
{
    public class Module
    {
        public string FullName { get; set; }
        public string Namespace { get; set; }
        public string Description { get; set; }
        public Nailhang.Significance Significance { get; set; }
        public string Assembly { get; set; }

        public ModuleInterface[] Interfaces { get; set; }
        public ModuleObject[] Objects { get; set; }
        public TypeReference[] ModuleBinds { get; set; }

        public string[] NamespaceDependencies { get; set; }
    }
}
