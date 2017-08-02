using Microsoft.AspNetCore.Mvc;
using Nailhang.IndexBase.Storage;
using Nailhang.Web.MD5Cache;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Nailhang.Web.Controllers
{
    public class InterfaceController : Controller
    {
        readonly IModulesStorage modulesStorage;
        readonly IMD5Cache md5Cache;

        public InterfaceController(IMD5Cache md5Cache, IModulesStorage modulesStorage)
        {
            this.md5Cache = md5Cache;
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
                .ToDictionary(w => md5Cache.ToMD5(w), w => w);

            var interfaceFullName = types[interfaceHash];            

            var dependentModules = allModules
                .Where(w =>
                w.Module.InterfaceDependencies.Any(q => q.FullName == interfaceFullName)
                || w.Module.ObjectDependencies.Any(q => q.FullName == interfaceFullName));

            var interfaceModules = allModules
                    .Where(w => w.Module
                                .ModuleBinds != null)
                    .Where(w => w.Module.ModuleBinds
                    .Any(q => q.FullName == interfaceFullName))
                    .ToArray();

            var interfaceItem = interfaceModules
                .SelectMany(q => q.Module.Interfaces)
                .Select(q => (Nailhang.IndexBase.ModuleInterface?)q)
                .Where(q => q.Value.TypeReference.FullName == interfaceFullName)
                .FirstOrDefault();

            var model = new Models.InterfaceModel
            {
                Name = interfaceFullName,
                InterfaceModules = interfaceModules,
                ModulesWithInterfaceDependencies = dependentModules,
                Interface = interfaceItem
            };
            return View("Index", model);
        }
    }
}