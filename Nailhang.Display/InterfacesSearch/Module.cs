using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.InterfacesSearch
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<Base.IInterfacesSearch>()
                .To<Processing.StorageSearch>();
        }
    }
}
