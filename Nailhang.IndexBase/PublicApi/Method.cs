using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.PublicApi
{
    public struct Method
    {
        public string Returns { get; set; }
        public string Name { get; set; }
        public Parameter[] Parameters { get; set; }
        public string[] GenericParameters { get; set; }
    }
}
