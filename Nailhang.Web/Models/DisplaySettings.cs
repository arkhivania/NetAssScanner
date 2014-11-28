using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Models
{
    public class DisplaySettings
    {
        public bool ShowDependencies { get; set; }
        public bool ShowObjects { get; set; }
        public bool ShowInterfaces { get; set; }

        public DisplaySettings()
        {
            this.ShowInterfaces = true;
        }
    }
}