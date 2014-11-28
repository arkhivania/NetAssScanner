using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.IndexBase.Index
{
    public interface IIndexProcessor
    {
        IEnumerable<Module> ExtractModules(string filePath);
    }
}
