using Nailhang.IndexBase.PublicApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Mongodb.PublicStorage.Processing
{
    class StoredAssembly
    {
        public string FullName { get; set; }
        public Class[] Classes { get; set; }
    }
}
