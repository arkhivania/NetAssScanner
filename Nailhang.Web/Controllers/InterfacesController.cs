using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Controllers
{
    public class InterfacesController : Controller
    {
        readonly IModulesStorage modulesStorage;

        public InterfacesController(IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }

        // GET: Interfaces
        public ActionResult Index()
        {
            var bindObjects = modulesStorage
                .GetModules()
                .Where(w => w.ModuleBinds != null)
                .SelectMany(w => w.ModuleBinds)
                .Select(w => w.FullName)
                .OrderBy(w => w)
                .Distinct()
                .ToArray();

            return View("Index", new Models.InterfacesModel { Interfaces = bindObjects });
        }
    }
}