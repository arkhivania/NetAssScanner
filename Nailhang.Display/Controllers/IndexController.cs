using Nailhang.Display.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Controllers
{
    public class IndexController
    {
        private readonly IndexBase.Storage.IModulesStorage modulesStorage;

        public IndexController(IndexBase.Storage.IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }

        public IndexModel Get(string rootNamespace)
        {
            var model = new IndexModel();
            var rootDeep = 3;            

            var allModules = modulesStorage.GetModules()
                                           .Select(w => new ModuleModel { Module = w })
                                           .ToArray();            
            

            model.Modules = allModules;
            model.AllModules = allModules;

            model.RootNamespaces = allModules
                                             .SelectMany(w => GetNamespaces(w.Module.FullName))
                                             .Distinct()
                                             .Where(w => w.Split('.').Length <= rootDeep)
                                             .OrderBy(w => w)
                                             .ToArray();

            if (!string.IsNullOrEmpty(model.SelectedRoot))
                model.Modules = allModules.Where(w => w.Module.FullName.StartsWith(model.SelectedRoot));
            return model;
        }

        private IEnumerable<string> GetNamespaces(string @namespace)
        {
            while (@namespace.Contains('.'))
                yield return @namespace = StringUtils.GetNamespace(@namespace);
        }
    }
}
