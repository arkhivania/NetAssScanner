using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.Storage
{
    [Flags]
    public enum ZoneType
    {
        Method = 0,
        Bootstrap = 1,
        Test = 2
    }
}
