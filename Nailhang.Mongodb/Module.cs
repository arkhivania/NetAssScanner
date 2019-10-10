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
using Nailhang.Mongodb.ModulesStorage.Processing;
using Nailhang.Mongodb.Base;

namespace Nailhang.Mongodb
{
    [Module]
    [ModuleDescription("Модуль хранения данных Nailhang в БД Mongo")]
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IMongoDatabase>()
                .ToMethod(q =>
                {
                    var mongoConnection = q.Kernel.Get<MongoConnection>();

                    var client = new MongoClient(mongoConnection.ConnectionString);
                    return client.GetDatabase(mongoConnection.DbName);
                });

            

            BsonSerializer.UseZeroIdChecker = true;
        }
    }
}
