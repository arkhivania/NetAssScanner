using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Controllers
{
    public class ModuleController : Controller
    {
        // GET: Module
        public ActionResult Index(string module)
        {
            return View();
        }
    }
}