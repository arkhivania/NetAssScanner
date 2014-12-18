using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Models
{
    public class IndexModel
    {
        public IEnumerable<ModuleModel> Modules { get; set; }
        public IEnumerable<ModuleModel> AllModules { get; set; }

        public IEnumerable<SelectListItem> RootNamespaces { get; set; }
        public string SelectedRoot { get; set; }

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