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

        public string[] GetNamespaces()
        {
            var controller = new IndexController(modulesStorage);
            var model = controller.Get("");

            return model.RootNamespaces.ToArray();
        }

        public ModuleModel[] GetModules(string baseNamespace)
        {
            var controller = new IndexController(modulesStorage);
            var model = controller.Get(baseNamespace);

            return model.Modules.ToArray();
        }
    }
}
