using System;

namespace Nailhang.Display.Models
{
    public struct InterfaceMD5KV
    {
        public string Name { get; set; }

        public int DepCount { get; set; }
        public Guid MD5 { get; set; }
    }
}