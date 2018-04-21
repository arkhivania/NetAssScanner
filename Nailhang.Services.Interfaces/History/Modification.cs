using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Services.Interfaces.History
{
    [Flags]
    public enum Modification
    {
        None = 0,
        Add = 1,
        Delete = 2,
        Modification = 4
    }
}
