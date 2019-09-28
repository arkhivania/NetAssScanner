using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Display.Models
{
    public struct ModulePageModel
    {
        public ModuleModel Module { get; set; }
        public IEnumerable<ChangesRow> Changes { get; set; }
    }
}
