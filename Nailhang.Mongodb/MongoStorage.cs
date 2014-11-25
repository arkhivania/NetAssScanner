using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb
{
    class MongoStorage : IModulesStorage
    {
        readonly MongoCollection<ModuleEntity> modules;

        public MongoStorage()
        {
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("nailhang_dev");
            this.modules = database.GetCollection<ModuleEntity>("modules");
        }

        public void StoreModule(IndexBase.Module module)
        {
            var mentity = new ModuleEntity{ Module = module, Id = module.FullName.GenerateGuid()};
            
            modules.Update(
                Query<ModuleEntity>.Where(w => w.Id == mentity.Id),
                        Update.Replace(mentity),
                        UpdateFlags.Upsert);
        }

        public IEnumerable<IndexBase.Module> GetModules()
        {
            foreach (var moduleEntity in modules.FindAll())
                yield return moduleEntity.Module;
        }
    }
}
