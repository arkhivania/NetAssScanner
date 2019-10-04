using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Tools.TextSearch.Base
{
    public interface IWSBuilder
    {
        IStat BuildStat(IEnumerable<string> text);
        IIndex Index(IEnumerable<Bulk> bulks, IStat stat);
        ISearch CreateSearch(IIndex index);
    }
}
