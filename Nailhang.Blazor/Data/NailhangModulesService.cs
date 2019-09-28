using Nailhang.Display.Controllers;
using Nailhang.Display.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Blazor.Data
{
    public class NailhangModulesService
    {
        private readonly IndexBase.Storage.IModulesStorage modulesStorage;
        private readonly InterfaceController interfaceController;

        public NailhangModulesService(IndexBase.Storage.IModulesStorage modulesStorage,
            InterfaceController interfaceController)
        {
            this.modulesStorage = modulesStorage;
            this.interfaceController = interfaceController;
        }

        public InterfaceModel GetInterface(Guid hash)
        {
            return interfaceController.GetModel(hash);
        }

        public ModuleModel GetModule(string moduleName)
        {
            var controller = new Display.Controllers.IndexController(modulesStorage);
            var model = controller.Get("");
            return model.Modules                
                .First(w => w.Module.FullName == moduleName);
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
