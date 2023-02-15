using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Processing.ZoneBuilder
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<Base.IZoneBuilder>().To<Processing.ZoneBuilder>();
        }
    }
}
