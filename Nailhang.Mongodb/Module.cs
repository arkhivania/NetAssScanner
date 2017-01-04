using MongoDB.Bson.Serialization;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Ninject;

namespace Nailhang.Mongodb
{
    [Nailhang.Module]
    [Nailhang.ModuleDescription("Модуль хранения данных Nailhang в БД Mongo")]
    public class Module : NinjectModule
    {
        public override void Load()
        {
            KernelConfiguration
                .Bind<MongoConnection>()
                .ToMethod(w =>
                {
                    //var builder = new ConfigurationBuilder()
                    //            .SetBasePath(Directory.GetCurrentDirectory())
                    //            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                    //var config = builder.Build();

                    var config = w.Kernel.Get<IConfiguration>();
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

                    return res;
                });

            KernelConfiguration
                .Bind<Nailhang.IndexBase.Storage.IModulesStorage>()
                .To<MongoStorage>();

            BsonSerializer.UseZeroIdChecker = true;
        }
    }
}
