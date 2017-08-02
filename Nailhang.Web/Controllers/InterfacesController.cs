using Microsoft.AspNetCore.Mvc;
using Nailhang.IndexBase.Storage;
using Nailhang.Web.MD5Cache;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nailhang.Web.Controllers
{
    public class InterfacesController : Controller
    {
        readonly IModulesStorage modulesStorage;
        readonly IMD5Cache md5Cache;

        public InterfacesController(IMD5Cache md5Cache, IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
            this.md5Cache = md5Cache;
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

            return View("Index", new Models.InterfacesModel
            {
                Interfaces = bindObjects
                .Select(q => new Models.InterfaceMD5KV
                {
                    Name = q,
                    MD5 = md5Cache.ToMD5(q)
                })
                .ToArray()
            });
        }
    }
}