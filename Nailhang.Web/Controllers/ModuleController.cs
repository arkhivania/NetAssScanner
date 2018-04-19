using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailhang.IndexBase.History.Base;
using Nailhang.IndexBase.Storage;
using Nailhang.Services.Interfaces;
using Newtonsoft.Json;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nailhang.Web.Controllers
{
    public class ModuleController : Controller
    {
        private readonly IModulesStorage modulesStorage;
        private readonly IGrainFactory grainFactory;

        public ModuleController(IModulesStorage modulesStorage, IGrainFactory grainFactory)
        {
            this.modulesStorage = modulesStorage;
            this.grainFactory = grainFactory;
        }        

        string GetNamespace(string module)
        {
            var msplit = module.Split(".");
            var nameSp = module;
            if (msplit.Length > 1)
                nameSp = string.Join(".", msplit.Take(msplit.Length - 1));
            return nameSp;
        }

        public async Task<ActionResult> Index(string module, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {
            var ns = GetNamespace(module);
            var changes = new Change[] { };
            if (ns != null)
            {
                var historyGrain = grainFactory.GetGrain<IModulesHistory>(ns);
                changes = await historyGrain.GetChanges();
            }            

            var groups = from c in changes
                         where (c.Modification & Modification.Modification) != 0
                         let m = new Models.MonthPeriod { Year = c.Revision.UtcDateTime.Year, Month = c.Revision.UtcDateTime.Month }
                         orderby m.Year * 12 + m.Month
                         group c by m;            

            var moduleModel = modulesStorage.GetModules()
                .Where(q => q.FullName == module)
                .Select(w => new Models.ModuleModel { Module = w })
                .First();
            
            return View("Index", new Models.ModulePageModel
            {
                Module = moduleModel,
                Changes = from g in groups
                          select new Models.ChangesRow
                          {
                              Month = g.Key,
                              Users = g.Select(q => q.Revision.User).Distinct().ToArray(),
                              Count = g.Count()
                          }
            });
        }
    }
}