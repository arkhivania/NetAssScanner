using Nailhang.Services.Interfaces.History;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Services.ModulesMarks.HotModulesBuilder.Base
{
    public struct HotInfo
    {
        public string Module { get; set;  }
        public Change[] LastRevisions { get; set; }
    }
}
