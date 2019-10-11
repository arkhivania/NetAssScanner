using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public struct AssemblyPublic
    {
        public string Id => $"{FullName}/{AssemblyVersion}";
        public Version AssemblyVersion { get; set; }
        public string FullName { get; set; }
        public Class[] Classes { get; set; }
        public string ShortName => FullName.Split(',')[0];
    }
}
