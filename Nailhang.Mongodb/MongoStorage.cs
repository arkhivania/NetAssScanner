using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using Nailhang.IndexBase.Storage;
using Newtonsoft.Json;
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

        readonly string dbName;

        public MongoStorage(MongoConnection mongoConnection)
        {
            var connectionString = mongoConnection.ConnectionString;
            this.dbName = mongoConnection.DbName;

            var mongoParam = Environment
                .GetCommandLineArgs()
                .FirstOrDefault(w => w.ToLower().StartsWith("-mongo:"));
            if(mongoParam != null)
                connectionString = mongoParam.Substring("-mongo:".Length);

            var client = new MongoClient(connectionString);
            var server = client.GetServer();

            

            var database = server.GetDatabase(dbName);
            this.modules = database.GetCollection<ModuleEntity>("modules");
        }

        public void StoreModule(IndexBase.Module module)
        {
            var mentity = new ModuleEntity
            {
                ModuleHeader = JsonConvert.SerializeObject(module),
                Id = module.FullName.GenerateGuid(),
                Namespace = module.Namespace,
                Name = module.FullName
            };
            
            modules.Update(
                Query<ModuleEntity>
                    .Where(w => w.Id == mentity.Id),
                        Update.Replace(mentity),
                        UpdateFlags.Upsert);
        }

        public IEnumerable<IndexBase.Module> GetModules()
        {
            foreach (var moduleEntity in modules.FindAll())
                yield return ToModule(moduleEntity.ModuleHeader);
        }

        IndexBase.Module ToModule(string header)
        {
            return JsonConvert.DeserializeObject<IndexBase.Module>(header);
        }

        public void DropModules(string namespaceStartsWith)
        {
            if (string.IsNullOrEmpty(namespaceStartsWith))
                modules.RemoveAll();
            else
                modules.Remove(Query<ModuleEntity>
                    .Where(w => w
                        .Name
                        .StartsWith(namespaceStartsWith)));
        }
    }
}
