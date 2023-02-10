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
        private readonly IndexController indexController;

        public NailhangModulesService(IndexBase.Storage.IModulesStorage modulesStorage,
            InterfaceController interfaceController, IndexController indexController)
        {
            this.modulesStorage = modulesStorage;
            this.interfaceController = interfaceController;
            this.indexController = indexController;
        }

        public InterfaceModel GetInterface(Guid hash)
        {
            return interfaceController.GetModel(hash);
        }

        public ModuleModel GetModule(string moduleName)
        {
            var model = indexController.Get("");
            return model.Modules
                .First(w => w.Module.FullName == moduleName);
        }

        public string[] GetNamespaces()
        {
            var model = indexController.Get("");

            return model.RootNamespaces.ToArray();
        }

        public ModuleModel[] GetModules(string baseNamespace)
        {
            var model = indexController.Get(baseNamespace);
            return model.Modules.ToArray();
        }
    }
}
