using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public struct AssemblyPublic
    {
        public string FullName { get; set; }
        public string Id => $"{FullName}";
        public string ShortName => FullName.Split(',')[0];
    }
}
