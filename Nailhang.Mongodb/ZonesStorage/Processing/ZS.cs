using MongoDB.Driver;
using Nailhang.IndexBase.PublicApi;
using Nailhang.IndexBase.Storage;
using Nailhang.Mongodb.ModulesStorage.Processing;
using Nailhang.Mongodb.PublicStorage.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Mongodb.ZonesStorage.Processing
{
    class ZS : IZonesStorage
    {
        private IMongoCollection<ZoneEntity> zonesCollection;

        public ZS(IMongoDatabase database)
        {
            zonesCollection = database.GetCollection<ZoneEntity>("zones");

            var indexOptions = new CreateIndexOptions();
            var indexKeys = Builders<ZoneEntity>.IndexKeys
                .Ascending(q => q.Id);

            var indexModel = new CreateIndexModel<ZoneEntity>(indexKeys, indexOptions);
            zonesCollection.Indexes.CreateOne(indexModel);
        }

        public long DropZones(string namespaceStartsWith)
        {
            var filter = Builders<ZoneEntity>.Filter.Empty;
            if (!string.IsNullOrEmpty(namespaceStartsWith))
            {
                filter = Builders<ZoneEntity>
                .Filter
                    .Where(t => t.Id.StartsWith(namespaceStartsWith));
            }

            var deleteResult = zonesCollection
                .DeleteMany(filter);
            return deleteResult.DeletedCount;
        }

        public IEnumerable<Zone> GetZones()
        {
            throw new NotImplementedException();
        }

        public void StoreZone(Zone zone)
        {
            var mentity = new ZoneEntity
            {
                ComponentIds = zone.ComponentIds,
                Id = zone.Path,
                Type = zone.Type, 
                Description = zone.Description
            };

            var filter = Builders<ZoneEntity>.Filter.Where(w => w.Id == mentity.Id);
            var replaceResult = zonesCollection.ReplaceOne(filter, mentity, new ReplaceOptions { IsUpsert = true });
        }
    }
}
