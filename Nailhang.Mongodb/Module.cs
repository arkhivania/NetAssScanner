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
using MongoDB.Driver;

namespace Nailhang.Mongodb
{
    [Nailhang.Module]
    [Nailhang.ModuleDescription("Модуль хранения данных Nailhang в БД Mongo")]
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel
                .Bind<MongoConnection>()
                .ToMethod(w =>
                {
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

            Kernel.Bind<IMongoDatabase>()
                .ToMethod(q =>
                {
                    var mongoConnection = q.Kernel.Get<MongoConnection>();

                    var connectionString = mongoConnection.ConnectionString;
                    var dbName = mongoConnection.DbName;

                    var mongoParam = Environment
                        .GetCommandLineArgs()
                        .FirstOrDefault(w => w.ToLower().StartsWith("-mongo:"));
                    if (mongoParam != null)
                        connectionString = mongoParam.Substring("-mongo:".Length);

                    var client = new MongoClient(connectionString);
                    return client.GetDatabase(dbName);
                });

            Kernel
                .Bind<Nailhang.IndexBase.Storage.IModulesStorage>()
                .To<MongoStorage>();

            BsonSerializer.UseZeroIdChecker = true;
        }
    }
}
