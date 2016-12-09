using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Nailhang.Web.Controllers
{
    public class ModuleController : Controller
    {
        private readonly Nailhang.IndexBase.Storage.IModulesStorage modulesStorage;

        public ModuleController(Nailhang.IndexBase.Storage.IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }        

        public ActionResult Index(string module, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {
            if (formUpdate)
                HttpContext.Session.SetString("DisplaySettings", JsonConvert.SerializeObject(displaySettings));

            var allModules = modulesStorage.GetModules()
                .Select(w => new Models.ModuleModel { Module = w })
                .ToArray();

            var model = allModules.First(w => w.Module.FullName == module);
            return View("Index", model);
        }
    }
}