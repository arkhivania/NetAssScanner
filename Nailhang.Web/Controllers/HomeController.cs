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

            var hotModules = new Dictionary<string, HotInfo>();
            for (int m = 0; m < 100; ++m)
                foreach (var h in await grainFactory.GetGrain<Nailhang.Services.ModulesMarks.OrleansHotModules.IHotModules>(m).GetInfos())
                {
                    if(h.LastRevisions.Length > 0)
                    if (string.IsNullOrEmpty(model.SelectedRoot)
                        || h.Module.StartsWith(model.SelectedRoot))
                    {
                        hotModules[h.Module] = h;
                    }
                }

            var allModules = modulesStorage.GetModules()
                                           .Select(w => new Models.ModuleModel { Module = w })
                                           .ToArray();

            var hotModuleModels = new List<HotModule>();
            foreach(var m in allModules)
            {
                HotInfo hotInfo;
                if(hotModules.TryGetValue(m.Namespace, out hotInfo))
                    hotModuleModels.Add(new HotModule { Module = m, HotInfo = hotInfo });
            }

            model.HotModules = hotModuleModels.OrderBy(q => q.Module.Module.FullName).ToArray();

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