using Nailhang.IndexBase.PublicApi;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Processing.PublicExtract
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IPublicProcessor>()
                .To<Processing.CecilExtract>();

            Kernel.Bind<Base.IClassExtractor>()
                .To<Processing.PublicExtractor>();
        }
    }
}
