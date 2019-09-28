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

        public IEnumerable<ModuleModel> ModulesWithDependencies
        {
            get
            {
                var allModules = AllModules.ToDictionary(w => w.Module.FullName);

                var dependencies = Modules.SelectMany(w => w.DependencyItems)
                    .Where(w => w.Module != null)
                    .Select(w => allModules[w.Module]);

                return Modules.Concat(dependencies).Distinct();
            }
        }
    }
}