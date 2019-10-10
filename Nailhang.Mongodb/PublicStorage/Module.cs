using Nailhang.IndexBase.PublicApi;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Mongodb.PublicStorage
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IPublicApiStorage>()
                .To<Processing.PublicStore>();
        }
    }
}
