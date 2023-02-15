using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.Storage
{
    public struct Zone
    {
        public string Path { get; set; }
        public string Description { get; set; }
        public string[] ComponentIds { get; set; }
        public ZoneType Type { get; set; }
    }
}
