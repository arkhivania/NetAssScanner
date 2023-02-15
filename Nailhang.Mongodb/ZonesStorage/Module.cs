using Nailhang.IndexBase.Storage;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Mongodb.ZonesStorage
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IZonesStorage>().To<Processing.ZS>().InSingletonScope();
        }
    }
}
