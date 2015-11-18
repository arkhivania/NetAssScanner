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

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session["DisplaySettings"] == null)
                Session["DisplaySettings"] = new Models.DisplaySettings();
        }

        public ActionResult Index(string module, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {
            if (formUpdate)
                Session["DisplaySettings"] = displaySettings;

            var allModules = modulesStorage.GetModules()
                .Select(w => new Models.ModuleModel { Module = w })
                .ToArray();

            var model = allModules.First(w => w.Module.FullName == module);
            return View("Index", model);
        }
    }
}