using Microsoft.AspNetCore.Mvc.Rendering;
using Nailhang.Services.ModulesMarks.HotModulesBuilder.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Nailhang.Web.Models
{
    public class IndexModel
    {
        public IndexModel()
        {            
            AllModules = new ModuleModel[] { };
            Modules = new ModuleModel[] { };
            RootNamespaces = new SelectListItem[] { };
        }

        public HotModule[] HotModules { get; set; } = new HotModule[] { };

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