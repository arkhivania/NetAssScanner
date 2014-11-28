using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Nailhang.IndexBase.Storage.IModulesStorage modulesStorage;

        public HomeController(Nailhang.IndexBase.Storage.IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }

        public ActionResult Index(Models.IndexModel model)
        {
            var rootDeep = Properties.Settings.Default.RootDeep;

            var allModules = modulesStorage.GetModules()
                .Select(w => new Models.ModuleModel { Module = w, DisplaySettings = model.DisplaySettings })
                .ToArray();
            model.Modules = allModules;
            model.AllModules = allModules;

            model.DisplaySettings.RootNamespaces = allModules
                    .SelectMany(w => GetNamespaces(w.Module.FullName)).Distinct()
                    .Where(w => w.Split('.').Length <= rootDeep)
                    .OrderBy(w => w)
                    .Select(w => new SelectListItem() { Value = w, Text = w })
                    .ToArray();

            if (!string.IsNullOrEmpty(model.DisplaySettings.SelectedRoot))
                model.Modules = allModules.Where(w => w.Module.FullName.StartsWith(model.DisplaySettings.SelectedRoot));
            
            return View(model);
        }

        private IEnumerable<string> GetNamespaces(string @namespace)
        {
            while(@namespace.Contains('.'))
                yield return @namespace = Utils.StringUtils.GetNamespace(@namespace);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Программа просмотра списка модулей";
            return View();
        }
    }
}