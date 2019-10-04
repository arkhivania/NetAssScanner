using Nailhang.Display.Tools.TextSearch.Base;
using Nailhang.Display.Tools.TextSearch.Processing;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Tools.TextSearch
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IWSBuilder>().To<StatBuilder>();
        }
    }
}
