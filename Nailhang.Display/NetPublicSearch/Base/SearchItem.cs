using Nailhang.IndexBase.PublicApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Base
{
    public struct SearchItem
    {
        public ClassType ClassType { get; set; }
        public string ClassName { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyVersion { get; set; }
        public string MethodName { get; set; }
        public string ParametersString { get; set; }
        public string GenericString { get; set; }
        public string ResultType { get; set; }
    }
}
