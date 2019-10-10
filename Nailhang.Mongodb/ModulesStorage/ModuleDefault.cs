using Microsoft.Extensions.Configuration;
using Nailhang.Mongodb.ModulesStorage.Base;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb.ModulesStorage
{
    public class ModuleDefault : NinjectModule
    {
        public override void Load()
        {
            Kernel
                .Bind<MongoConnection>()
                .ToMethod(sp =>
                {
                    var config = sp.Kernel.Get<IConfiguration>();
                    var section = config.GetSection("Mongo");
                    var res = new MongoConnection
                    {
                        ConnectionString = section["ConnectionString"],
                        DbName = section["DbName"]
                    };

                    if (res.ConnectionString == null)
                        res.ConnectionString = "mongodb://localhost";

                    if (res.DbName == null)
                        res.DbName = "nailhang";

                    var mongoParam = Environment
                        .GetCommandLineArgs()
                        .FirstOrDefault(w => w.ToLower().StartsWith("-mongo:"));
                    if (mongoParam != null)
                        res.ConnectionString = mongoParam.Substring("-mongo:".Length);

                    return res;
                });
        }
    }
}
