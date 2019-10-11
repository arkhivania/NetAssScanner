using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Display.NetPublicSearch
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<Processing.NetSearch>()
                .ToSelf();

            Kernel.Bind<Base.INetSearch>().To<Processing.NetSearch>()
                .InSingletonScope();
                
        }
    }
}
