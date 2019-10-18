using Nailhang.Mongodb.Base;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IntegrationTests.TestModules
{
    class LocalMongo : NinjectModule
    {
        public override void Load()
        {
            Kernel
                .Bind<MongoConnection>()
                .ToMethod(sp =>
                {   
                    var res = new MongoConnection
                    {
                        ConnectionString = "mongodb://127.0.0.1",
                        DbName = "nailhang"
                    };

                    return res;
                });
        }
    }
}
