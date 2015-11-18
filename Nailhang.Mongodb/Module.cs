using MongoDB.Bson.Serialization;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    var configSection = ConfigurationManager.GetSection("mongoConnection");
                    if (configSection != null)
                        return (MongoConnection)configSection;

                    return new MongoConnection();
                });

            Kernel.Bind<Nailhang.IndexBase.Storage.IModulesStorage>().To<MongoStorage>();

            BsonSerializer.UseZeroIdChecker = true;

            //BsonClassMap.RegisterClassMap<IndexBase.TypeReference>(cm => {
            //    cm.MapProperty(c => c.AssemblyName);
            //    cm.MapProperty(c => c.FullName);
            //});
        }
    }
}
