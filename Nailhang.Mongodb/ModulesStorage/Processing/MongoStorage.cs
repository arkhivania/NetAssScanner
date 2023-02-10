using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nailhang.IndexBase.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb.ModulesStorage.Processing
{
    class MongoStorage : IModulesStorage
    {
        readonly IMongoCollection<ModuleEntity> modules;

        public MongoStorage(IMongoDatabase database)
        {
            modules = database.GetCollection<ModuleEntity>("modules");
        }

        public void StoreModule(IndexBase.Module module)
        {
            var mentity = new ModuleEntity
            {
                ModuleHeader = JsonConvert.SerializeObject(module),
                Id = module.FullName.GenerateGuid(),
                FullName = module.FullName
            };

            var filter = Builders<ModuleEntity>.Filter.Where(w => w.Id == mentity.Id);
            var replaceResult = modules.ReplaceOne(filter, mentity, new ReplaceOptions { IsUpsert = true });
        }

        public IEnumerable<IndexBase.Module> GetModules()
        {
            foreach (var moduleEntity in modules.AsQueryable())
                yield return ToModule(moduleEntity.ModuleHeader);
        }

        IndexBase.Module ToModule(string header)
        {
            return JsonConvert.DeserializeObject<IndexBase.Module>(header);
        }

        public void DropModules(string namespaceStartsWith)
        {
            if (string.IsNullOrEmpty(namespaceStartsWith))
                modules.DeleteMany(new MongoDB.Bson.BsonDocument());
            else
                modules.DeleteMany(Builders<ModuleEntity>.Filter
                    .Where(w => w
                        .FullName
                        .StartsWith(namespaceStartsWith)));
        }
    }
}
