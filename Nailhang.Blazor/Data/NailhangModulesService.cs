using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Blazor.Data
{
    public class NailhangModulesService
    {
        private readonly IndexBase.Storage.IModulesStorage modulesStorage;

        public NailhangModulesService(IndexBase.Storage.IModulesStorage modulesStorage)
        {
            this.modulesStorage = modulesStorage;
        }

        public Task<string[]> GetNamespaces()
        {
            var controller = new Display.Controllers.IndexController(modulesStorage);
            var model = controller.Get("");            

            return Task.FromResult(model.RootNamespaces.ToArray());
        }

        public Task<Nailhang.Display.Models.ModuleModel[]> GetModules(string baseNamespace)
        {
            var controller = new Display.Controllers.IndexController(modulesStorage);
            var model = controller.Get(baseNamespace);
            
            return Task.FromResult(model.Modules.ToArray());
        }
    }
}
