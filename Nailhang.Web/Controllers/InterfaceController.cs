using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Controllers
{
    public class InterfaceController : Controller
    {
        readonly IModulesStorage modulesStorage;

        public InterfaceController(IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }

        // GET: Interface
        public ActionResult Index(Guid interfaceHash)
        {
            var allModules = modulesStorage.GetModules()
                .Select(w => new Models.ModuleModel { Module = w })
                .ToArray();

            var types = allModules
                .Where(w => w.Module.ModuleBinds != null)
                .SelectMany(w => w.Module.ModuleBinds)
                .Select(w => w.FullName)
                .Distinct()
                .ToDictionary(w => w.ToMD5(), w => w);

            HomeController.CreateDependencies(allModules);

            var model = new Models.InterfaceModel
            {
                Name = types[interfaceHash],
                InterfaceModules = allModules
                    .Where(w => w.Module
                                .ModuleBinds != null)
                    .Where(w => w.Module.ModuleBinds
                    .Any(q => q.FullName.ToMD5().Equals(interfaceHash)))
                    .ToArray()
            };
            return View("Index", model);
        }
    }
}