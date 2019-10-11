using MongoDB.Driver;
using Nailhang.IndexBase.PublicApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Mongodb.PublicStorage.Processing
{
    class PublicStore : IPublicApiStorage
    {
        private readonly IMongoDatabase database;
        readonly IMongoCollection<AssemblyEntity> assembliesCollection;

        public PublicStore(IMongoDatabase database)
        {
            this.database = database;
            assembliesCollection = database.GetCollection<AssemblyEntity>("publicAssemblies");

            var indexOptions = new CreateIndexOptions();
            var indexKeys = Builders<AssemblyEntity>.IndexKeys
                .Ascending(q => q.StringId);

            var indexModel = new CreateIndexModel<AssemblyEntity>(indexKeys, indexOptions);
            assembliesCollection.Indexes.CreateOne(indexModel);
        }

        public IEnumerable<AssemblyPublic> LoadAssemblies()
        {
            foreach (var entity in assembliesCollection.AsQueryable())
                yield return ToAssemblyPublic(entity.Data);
        }

        private AssemblyPublic ToAssemblyPublic(string data)
        {
            return JsonConvert.DeserializeObject<AssemblyPublic>(data);
        }

        public void UpdateAssemblies(IEnumerable<AssemblyPublic> assemblies)
        {
            foreach(var a in assemblies)
            {
                var entity = new AssemblyEntity
                {
                    Data = JsonConvert.SerializeObject(a),
                    Id = a.Id.GenerateGuid(), 
                    StringId = a.Id
                };

                var filter = Builders<AssemblyEntity>.Filter.Where(w => w.Id == entity.Id);
                var replaceResult = assembliesCollection
                    .ReplaceOne(filter, entity, new UpdateOptions { IsUpsert = true });
            }
        }

        public IEnumerable<AssemblyPublic> LoadAssembly(string fullName)
        {
            foreach (var entity in assembliesCollection
                .AsQueryable()
                .Where(q => q.StringId.StartsWith(fullName)))
            {
                yield return ToAssemblyPublic(entity.Data);
            }
        }
    }
}
