﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.IndexBase
{
    public struct ModuleInterface
    {
        public TypeReference TypeReference { get; set; }
        public string Description { get; set; }
        public InterfaceMethod[] Methods { get; set; }
    }
}
