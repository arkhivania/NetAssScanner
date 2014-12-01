using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb
{
    class MongoStorage : IModulesStorage
    {
        readonly MongoCollection<ModuleEntity> modules;

        string dbName = "nailhang";

        public MongoStorage()
        {
            var connectionString = "mongodb://localhost";

            var mongoParam = Environment.GetCommandLineArgs().FirstOrDefault(w => w.ToLower().StartsWith("-mongo:"));
            if(mongoParam != null)
                connectionString = mongoParam.Substring("-mongo:".Length);

            var client = new MongoClient(connectionString);
            var server = client.GetServer();

            InitDebug();

            var database = server.GetDatabase(dbName);
            this.modules = database.GetCollection<ModuleEntity>("modules");
        }

        [Conditional("DB_DEBUG")]
        void InitDebug()
        {
            this.dbName = "nailhang_dev";
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


        public void DropModules(string namespaceStartsWith)
        {
            if (string.IsNullOrEmpty(namespaceStartsWith))
                modules.RemoveAll();
            else
                modules.Remove(Query<ModuleEntity>.Where(w => w.Module.FullName.StartsWith(namespaceStartsWith)));            
        }
    }
}
