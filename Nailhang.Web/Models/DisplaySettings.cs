using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nailhang.Web.Models
{
    public class DisplaySettings
    {
        public bool ShowDependencies { get; set; } = true;
        public bool ShowBinds { get; set; } = true;
        public bool ShowObjects { get; set; } = true;
        public bool ShowInterfaces { get; set; } = true;
    }
}