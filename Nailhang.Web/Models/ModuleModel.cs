﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nailhang.Web.Models
{
    public class ModuleModel
    {
        public Nailhang.IndexBase.Module Module { get; set; }

        public string Namespace
        {
            get 
            {
                return Module.Namespace;
            }
        }

        public DependencyItem[] DependencyItems { get; set; }
        public DependencyItem[] ItemsWithThisDependency { get; set; }
    }
}