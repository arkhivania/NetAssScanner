using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.Storage
{
    [Flags]
    public enum ZoneType
    {
        Method = 1,
        Bootstrap = 2,
        Test = 4
    }
}
