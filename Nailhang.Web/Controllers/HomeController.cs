using System;
using System.Collections.Generic;
using System.Linq;
using Nailhang.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var dsString = HttpContext.Session.GetString("DisplaySettings");
            if(string.IsNullOrEmpty(dsString))
                return PartialView("DisplaySettings", new DisplaySettings());

            var ds = JsonConvert.DeserializeObject<DisplaySettings>(dsString);
            return PartialView("DisplaySettings", ds);
        }

        public ActionResult Map(Models.IndexModel model, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {
            if (formUpdate)
                HttpContext.Session.SetString("DisplaySettings", JsonConvert.SerializeObject(displaySettings));

            UpdateIndexModel(model, displaySettings);

            return View(model);
        }

        public ActionResult Index(Models.IndexModel model, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {
            ValidateSession();
            if(formUpdate)
                HttpContext.Session.SetString("DisplaySettings", JsonConvert.SerializeObject(displaySettings));

            UpdateIndexModel(model, displaySettings);
            
            return View(model);
        }

        private void ValidateSession()
        {
            if(HttpContext.Session.GetString("DisplaySettings") == null)
                HttpContext.Session.SetString("DisplaySettings", JsonConvert.SerializeObject(new DisplaySettings()));
        }

        private void UpdateIndexModel(IndexModel model, Models.DisplaySettings displaySettings)
        {
            var rootDeep = 3;

            var allModules = modulesStorage.GetModules()
                                           .Select(w => new Models.ModuleModel { Module = w })
                                           .ToArray();

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