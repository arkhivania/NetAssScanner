using System;
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

            UpdateIndexModel(model, displaySettings);

            return View(model);
        }

        public ActionResult Index(Models.IndexModel model, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {
            if(formUpdate)
                Session["DisplaySettings"] = displaySettings;

            UpdateIndexModel(model, displaySettings);
            
            return View(model);
        }

        private void UpdateIndexModel(IndexModel model, Models.DisplaySettings displaySettings)
        {
            var rootDeep = Properties.Settings.Default.RootDeep;

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