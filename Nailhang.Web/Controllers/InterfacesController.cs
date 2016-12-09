using Microsoft.AspNetCore.Mvc;
using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        public ActionResult Index(Models.InterfacesModel model)
        {
            var bindObjects = modulesStorage
                .GetModules()
                .Where(w => w.ModuleBinds != null)
                .SelectMany(w => w.ModuleBinds)
                .Select(w => w.FullName)
                .OrderBy(w => w)
                .Distinct();



            if (!string.IsNullOrEmpty(model.Contains))
            {
                var query = model.Contains.ToLower();
                bindObjects = bindObjects.Where(w => w.ToLower().Contains(query));
            }

            return View("Index", new Models.InterfacesModel { Interfaces = bindObjects.ToArray() });
        }
    }
}