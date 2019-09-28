using Nailhang.IndexBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nailhang.Display.Models
{
    public class InterfaceModel
    {
        public string Name { get; set; }

        public ModuleInterface? Interface { get; set; }

        public IEnumerable<ModuleModel> InterfaceModules { get; set; }
        public IEnumerable<ModuleModel> ModulesWithInterfaceDependencies { get; set; }
    }
}