using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailhang.IndexBase.History.Base;
using Nailhang.IndexBase.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nailhang.Web.Controllers
{
    public class ModuleController : Controller
    {
        private readonly IModulesStorage modulesStorage;
        private readonly IHistoryStorage historyStorage;

        public ModuleController(IModulesStorage modulesStorage, IHistoryStorage historyStorage)
        {
            this.modulesStorage = modulesStorage;
            this.historyStorage = historyStorage;
        }        

        string GetNamespace(string module)
        {
            var msplit = module.Split(".");
            var nameSp = module;
            if (msplit.Length > 1)
                nameSp = string.Join(".", msplit.Take(msplit.Length - 1));
            return nameSp;
        }

        public ActionResult Index(string module, Models.DisplaySettings displaySettings, bool formUpdate = false)
        {   
            var changes = historyStorage.GetChanges(GetNamespace(module));

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