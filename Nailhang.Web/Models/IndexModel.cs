using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Models
{
    public class IndexModel
    {
        public IEnumerable<ModuleModel> Modules { get; set; }
        public IEnumerable<ModuleModel> AllModules { get; set; }

        public IEnumerable<SelectListItem> RootNamespaces { get; set; }
        public string SelectedRoot { get; set; }


        public DisplaySettings DisplaySettings { get; set; }        

        public IndexModel()
        {
            this.DisplaySettings = new DisplaySettings();
        }
    }
}