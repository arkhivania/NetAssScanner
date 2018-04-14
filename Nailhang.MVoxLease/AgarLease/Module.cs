using Nailhang.Processing.ModuleBuilder.Base;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.MVoxLease.AgarLease
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IModuleBuilder>().To<Processing.AgarTypesProcessing>();
        }
    }
}
