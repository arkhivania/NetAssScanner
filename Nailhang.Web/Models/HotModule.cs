using Nailhang.Services.ModulesMarks.HotModulesBuilder.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Web.Models
{
    public class HotModule
    {
        public HotInfo HotInfo { get; set; }
        public ModuleModel Module { get; set; }
    }
}
