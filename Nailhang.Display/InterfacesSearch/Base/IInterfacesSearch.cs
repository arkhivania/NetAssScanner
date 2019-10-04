using Nailhang.Display.Models;
using Nailhang.Display.Tools.TextSearch.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.InterfacesSearch.Base
{
    public interface IInterfacesSearch
    {
        IEnumerable<InterfaceMD5KV> Search(string name);
    }
}
