using System;
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

        public ActionResult Index()
        {
            var modules = modulesStorage.GetModules();
            return View(modules);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Программа просмотра списка модулей";
            return View();
        }
    }
}