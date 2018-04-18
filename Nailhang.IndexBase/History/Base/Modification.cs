using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.History.Base
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
