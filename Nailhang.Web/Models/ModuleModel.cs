using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nailhang.Web.Models
{
    public class ModuleModel
    {
        public DisplaySettings DisplaySettings { get; set; }
        public Nailhang.IndexBase.Module Module { get; set; }

        public string Namespace
        {
            get 
            {
                return Utils.StringUtils.GetNamespace(Module.FullName);
            }
        }
    }
}