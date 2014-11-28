﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Nailhang.IndexBase.Storage.IModulesStorage modulesStorage;

        public HomeController(Nailhang.IndexBase.Storage.IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }

        public ActionResult Index(Models.IndexModel model, bool formUpdate = false)
        {
            if (formUpdate)
                Session["DisplaySettings"] = model.DisplaySettings;

            model.DisplaySettings = (Models.DisplaySettings)Session["DisplaySettings"];
            if (model.DisplaySettings == null)
                model.DisplaySettings = new Models.DisplaySettings() { ShowDependencies = true, ShowInterfaces = true, ShowObjects = true };

            var rootDeep = Properties.Settings.Default.RootDeep;

            var allModules = modulesStorage.GetModules()
                .Select(w => new Models.ModuleModel { Module = w, DisplaySettings = model.DisplaySettings })
                .ToArray();

            CreateDependencies(allModules);

            model.Modules = allModules;
            model.AllModules = allModules;

            model.RootNamespaces = allModules
                    .SelectMany(w => GetNamespaces(w.Module.FullName)).Distinct()
                    .Where(w => w.Split('.').Length <= rootDeep)
                    .OrderBy(w => w)
                    .Select(w => new SelectListItem() { Value = w, Text = w })
                    .ToArray();

            if (!string.IsNullOrEmpty(model.SelectedRoot))
                model.Modules = allModules.Where(w => w.Module.FullName.StartsWith(model.SelectedRoot));

            
            
            return View(model);
        }

        internal static void CreateDependencies(Models.ModuleModel[] modules)
        {
            foreach(var m in modules)
            {
                m.DependencyItems = m.Module.NamespaceDependencies.Select(w => new Models.DependencyItem { Name = w }).ToArray();

                foreach(var depItem in m.DependencyItems)
                {
                    var moduleReference = modules.FirstOrDefault(w => depItem.Name.StartsWith(w.Namespace));
                    if (moduleReference != null)
                        depItem.Module = moduleReference.Module.FullName;
                }
            }
        }

        private IEnumerable<string> GetNamespaces(string @namespace)
        {
            while(@namespace.Contains('.'))
                yield return @namespace = Utils.StringUtils.GetNamespace(@namespace);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Программа просмотра списка модулей";
            return View();
        }
    }
}