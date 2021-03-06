﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.IndexBase
{
    public class Module
    {
        public string FullName { get; set; }
        
        public string Description { get; set; }
        public Nailhang.Significance Significance { get; set; }
        public string Assembly { get; set; }

        public ModuleInterface[] Interfaces { get; set; } = new ModuleInterface[] { };
        public ModuleObject[] Objects { get; set; } = new ModuleObject[] { };
        public TypeReference[] ModuleBinds { get; set; } = new TypeReference[] { };

        public TypeReference[] InterfaceDependencies { get; set; } = new TypeReference[] { };
        public TypeReference[] ObjectDependencies { get; set; } = new TypeReference[] { };

        public string[] Tags { get; set; } = new string[] { };
    }
}
