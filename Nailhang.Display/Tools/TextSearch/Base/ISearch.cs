using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Tools.TextSearch.Base
{
    public interface ISearch
    {
        IEnumerable<Response> Search(Request request);
    }
}
