using Nailhang.IndexBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nailhang.Web.Models
{
    public class ModuleModel
    {
        public Nailhang.IndexBase.Module Module { get; set; }

        public string Namespace
        {
            get 
            {
                return Module.FullName.ToNamespace();
            }
        }

        public DependencyItem[] DependencyItems { get; set; } = new DependencyItem[] { };
        public DependencyItem[] ItemsWithThisDependency { get; set; } = new DependencyItem[] { };
    }
}