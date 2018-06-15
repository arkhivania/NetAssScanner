using System;
using System.Collections.Generic;
using System.Linq;
using Nailhang.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Orleans;
using System.Threading.Tasks;
using Nailhang.Services.ModulesMarks.HotModulesBuilder.Base;

namespace Nailhang.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Nailhang.IndexBase.Storage.IModulesStorage modulesStorage;
        private readonly IGrainFactory grainFactory;

        public HomeController(Nailhang.IndexBase.Storage.IModulesStorage modulesStorage, IGrainFactory grainFactory)
        {
            this.modulesStorage = modulesStorage;
            this.grainFactory = grainFactory;
        }

        public async Task<ActionResult> Index(Models.IndexModel model, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {
            model = await UpdateIndexModel(model, displaySettings);            
            return View(model);
        }

        private async Task<IndexModel> UpdateIndexModel(IndexModel model, Models.DisplaySettings displaySettings)
        {
            var rootDeep = 3;

            var hotModules = new List<HotInfo>();
            for (int m = 0; m < 100; ++m)
                hotModules.AddRange(await grainFactory.GetGrain<Nailhang.Services.ModulesMarks.OrleansHotModules.IHotModules>(m).GetInfos());

            if (!string.IsNullOrEmpty(model.SelectedRoot))
                hotModules.RemoveAll(q => !q.Module.StartsWith(model.SelectedRoot));

            model.HotModules = hotModules
                .Where(q => q.LastRevisions.Length > 0)
                .OrderByDescending(q => q.LastRevisions.Select(q2 => q2.Revision.UtcDateTime).Last())
                .ToArray();

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
            return model;
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