using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Controllers
{
    public class ModuleController : Controller
    {
        private readonly Nailhang.IndexBase.Storage.IModulesStorage modulesStorage;

        public ModuleController(Nailhang.IndexBase.Storage.IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }

        public ActionResult Index(Models.ModuleModel model, string module)
        {
            var allModules = modulesStorage.GetModules()
                .Select(w => new Models.ModuleModel { Module = w, DisplaySettings = model.DisplaySettings })
                .ToArray();

            HomeController.CreateDependencies(allModules);

            var targetModel = allModules.First(w => w.Module.FullName == module);
            model.Module = targetModel.Module;
            model.DependencyItems = targetModel.DependencyItems;
            return View(model);
        }
    }
}