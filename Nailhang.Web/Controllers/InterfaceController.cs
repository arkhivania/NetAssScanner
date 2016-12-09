using Microsoft.AspNetCore.Mvc;
using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;


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

            var interfaceFullName = types[interfaceHash];

            var dependentModules = allModules
                .Where(w =>
                w.Module.InterfaceDependencies.Any(q => q.FullName == interfaceFullName)
                || w.Module.ObjectDependencies.Any(q => q.FullName == interfaceFullName));

            var model = new Models.InterfaceModel
            {
                Name = interfaceFullName,
                InterfaceModules = allModules
                    .Where(w => w.Module
                                .ModuleBinds != null)
                    .Where(w => w.Module.ModuleBinds
                    .Any(q => q.FullName.ToMD5().Equals(interfaceHash)))
                    .ToArray(),
                ModulesWithInterfaceDependencies = dependentModules
            };
            return View("Index", model);
        }
    }
}