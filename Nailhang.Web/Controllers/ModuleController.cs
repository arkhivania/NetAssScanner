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

        public ActionResult Index(Models.ModuleModel model, string module, bool formUpdate = false)
        {
            if (formUpdate)
                Session["DisplaySettings"] = model.DisplaySettings;

            var allModules = modulesStorage.GetModules()
                .Select(w => new Models.ModuleModel { Module = w, DisplaySettings = model.DisplaySettings })
                .ToArray();
            
            model.DisplaySettings = (Models.DisplaySettings)Session["DisplaySettings"];
            if (model.DisplaySettings == null)
                model.DisplaySettings = new Models.DisplaySettings() { ShowDependencies = true, ShowInterfaces = true, ShowObjects = true  };

            HomeController.CreateDependencies(allModules);

            var targetModel = allModules.First(w => w.Module.FullName == module);
            model.Module = targetModel.Module;
            model.DependencyItems = targetModel.DependencyItems;
            return View("Index", model);
        }
    }
}