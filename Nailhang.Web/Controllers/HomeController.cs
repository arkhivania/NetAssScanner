﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nailhang.Web.Models;

namespace Nailhang.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Nailhang.IndexBase.Storage.IModulesStorage modulesStorage;

        public HomeController(Nailhang.IndexBase.Storage.IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }

        public PartialViewResult RenderParameters()
        {
            return PartialView("DisplaySettings", Session["DisplaySettings"]);
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (Session["DisplaySettings"] == null)
                Session["DisplaySettings"] = new Models.DisplaySettings();
        }

        public ActionResult Map(Models.IndexModel model, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {
            if (formUpdate)
                Session["DisplaySettings"] = displaySettings;

            UpdateIndexModel(model);

            return View(model);
        }

        public ActionResult Index(Models.IndexModel model, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {
            if(formUpdate)
                Session["DisplaySettings"] = displaySettings;

            UpdateIndexModel(model);
            
            return View(model);
        }
 
        private void UpdateIndexModel(IndexModel model)
        {
            var rootDeep = Properties.Settings.Default.RootDeep;

            var allModules = modulesStorage.GetModules()
                                           .Select(w => new Models.ModuleModel { Module = w })
                                           .ToArray();

            CreateDependencies(allModules);

            model.Modules = allModules;
            model.AllModules = allModules;

            model.RootNamespaces = allModules
                                             .SelectMany(w => GetNamespaces(w.Module.FullName))
                                             .Distinct()
                                             .Where(w => w.Split('.').Length <= rootDeep)
                                             .OrderBy(w => w)
                                             .Select(w => new SelectListItem()
                                                    {
                                                        Value = w,
                                                        Text = w
                                                    })
                                             .ToArray();

            if (!string.IsNullOrEmpty(model.SelectedRoot))
                model.Modules = allModules.Where(w => w.Module.FullName.StartsWith(model.SelectedRoot));
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