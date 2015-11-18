using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Controllers
{
    public class InterfacesController : Controller
    {
        readonly IModulesStorage modulesStorage;

        public InterfacesController(IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (Session["DisplaySettings"] == null)
                Session["DisplaySettings"] = new Models.DisplaySettings();
        }

        // GET: Interfaces
        public ActionResult Index(Models.InterfacesModel model)
        {
            var bindObjects = modulesStorage
                .GetModules()
                .Where(w => w.ModuleBinds != null)
                .SelectMany(w => w.ModuleBinds)
                .Select(w => w.FullName)
                .OrderBy(w => w)
                .Distinct();



            if (!string.IsNullOrEmpty(model.Contains))
            {
                var query = model.Contains.ToLower(CultureInfo.CurrentCulture);
                bindObjects = bindObjects.Where(w => w.ToLower(CultureInfo.CurrentCulture).Contains(query));
            }

            return View("Index", new Models.InterfacesModel { Interfaces = bindObjects.ToArray() });
        }
    }
}