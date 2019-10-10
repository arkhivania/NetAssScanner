using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Mongodb.ModulesStorage
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel
                .Bind<IndexBase.Storage.IModulesStorage>()
                .To<Processing.MongoStorage>();
        }
    }
}
