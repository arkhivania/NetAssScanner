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

        /// <summary>
        /// Очищает модули, namespace которых начинается с параметра namespaceStartsWith
        /// </summary>
        /// <param name="namespaceStartsWith">С чего начинается namespace для удаления. Если пустой, удаляются все модули в базе</param>
        void DropModules(string namespaceStartsWith);
    }
}
