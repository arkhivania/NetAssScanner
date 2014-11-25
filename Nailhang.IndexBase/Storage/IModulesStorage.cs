using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.IndexBase.Storage
{
    public interface IModulesStorage
    {
        void StoreModule(Module module);
        IEnumerable<Module> GetModules();
    }
}
