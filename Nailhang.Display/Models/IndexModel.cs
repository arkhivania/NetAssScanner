using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Nailhang.Display.Models
{
    public class IndexModel
    {
        public IndexModel()
        {
            AllModules = new ModuleModel[] { };
            Modules = new ModuleModel[] { };
            RootNamespaces = new string[] { };
        }

        public IEnumerable<ModuleModel> Modules { get; set; }
        public IEnumerable<ModuleModel> AllModules { get; set; }
        public IEnumerable<string> RootNamespaces { get; set; }
    }
}