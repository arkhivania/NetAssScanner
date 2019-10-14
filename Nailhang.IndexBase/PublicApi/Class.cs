using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public struct Class
    {
        public string Name { get; set; }
        public Method[] Methods { get; set; }
        public ClassType ClassType { get; set; }
        public string Namespace { get; set; }
        public string FullName => $"{Namespace}.{Name}";
        public bool IsPublic { get; set; }
    }
}
