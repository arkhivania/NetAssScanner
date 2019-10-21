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
        readonly IMongoCollection<AssemblyEntity> assembliesCollection;

        public PublicStore(IMongoDatabase database)
        {
            assembliesCollection = database.GetCollection<AssemblyEntity>("publicAssemblies");

            var indexOptions = new CreateIndexOptions();
            var indexKeys = Builders<AssemblyEntity>.IndexKeys
                .Ascending(q => q.StringId);

            var indexModel = new CreateIndexModel<AssemblyEntity>(indexKeys, indexOptions);
            assembliesCollection.Indexes.CreateOne(indexModel);
        }

        IEnumerable<StoredAssembly> LoadAssemblies()
        {
            foreach (var entity in assembliesCollection.AsQueryable())
                yield return ToAssemblyPublic(entity.Data);
        }

        private StoredAssembly ToAssemblyPublic(string data)
        {
            return JsonConvert.DeserializeObject<StoredAssembly>(data);
        }

        public void UpdateAssemblies(IEnumerable<(AssemblyPublic, Class[])> assemblies)
        {
            foreach (var a in assemblies)
            {
                var entity = new AssemblyEntity
                {
                    Data = JsonConvert.SerializeObject(new StoredAssembly
                    {
                        Classes = a.Item2,
                        FullName = a.Item1.FullName
                    }),
                    Id = a.Item1.Id.GenerateGuid(),
                    StringId = a.Item1.Id
                };

                var filter = Builders<AssemblyEntity>.Filter.Where(w => w.Id == entity.Id);
                var replaceResult = assembliesCollection
                    .ReplaceOne(filter, entity, new UpdateOptions { IsUpsert = true });
            }
        }

        IEnumerable<StoredAssembly> LoadAssembly(string namestarts)
        {
            foreach (var entity in assembliesCollection
                .AsQueryable()
                .Where(q => q.StringId.StartsWith(namestarts)))
            {
                yield return ToAssemblyPublic(entity.Data);
            }
        }

        public long Drop(DropRequest dropRequest)
        {
            var filter = Builders<AssemblyEntity>.Filter.Empty;
            if (!string.IsNullOrEmpty(dropRequest.NameStartsWith))
            {
                filter = Builders<AssemblyEntity>
                    .Filter
                    .Where(t => t.StringId.StartsWith(dropRequest.NameStartsWith));
            }

            var deleteResult = assembliesCollection
                .DeleteMany(filter);
            return deleteResult.DeletedCount;
        }

        public IEnumerable<Class> LoadClasses(string assemblyId)
        {
            var f = assembliesCollection.AsQueryable()
                    .Where(q => q.StringId == assemblyId).FirstOrDefault();
            return ToAssemblyPublic(f.Data).Classes;
        }

        IEnumerable<AssemblyPublic> IPublicApiStorage.LoadAssemblies()
        {
            foreach (var a in LoadAssemblies())
                yield return new AssemblyPublic { FullName = a.FullName };
        }

        AssemblyPublic? IPublicApiStorage.LoadAssembly(string fullName)
        {
            return LoadAssembly(fullName)
                .Select(w => (AssemblyPublic?)new AssemblyPublic { FullName = w.FullName })
                .FirstOrDefault();
        }

        public IEnumerable<AssemblyPublic> FindByShortName(string shortName)
        {
            return LoadAssembly(shortName)
                .Select(w => new AssemblyPublic { FullName = w.FullName });
        }
    }
}
