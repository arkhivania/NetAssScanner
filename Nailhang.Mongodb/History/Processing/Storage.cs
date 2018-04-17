using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Nailhang.IndexBase.History.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Mongodb.History.Processing
{
    class Storage : IHistoryStorage
    {
        readonly IMongoCollection<Namespace> namespaces;
        
        private readonly IMongoDatabase mongoDatabase;

        public Storage(IMongoDatabase mongoDatabase)
        {
            this.mongoDatabase = mongoDatabase;
            namespaces = mongoDatabase.GetCollection<Namespace>("namespaces");            
        }

        ProcInfo GetProcInfo()
        {
            var pcoll = mongoDatabase.GetCollection<ProcInfo>("procInfo");
            return pcoll.AsQueryable<ProcInfo>().FirstOrDefault();
        }

        public int LastRevision => GetProcInfo().LastRevision;

        public IEnumerable<Revision> GetChanges(string @namespace)
        {
            var nspace = namespaces.Find(q => q.Id == @namespace).FirstOrDefault();
            if (nspace.Revisions != null)
                return nspace.Revisions;
            return Enumerable.Empty<Revision>();
        }

        public void StoreChangeToNamespace(string @namespace, Revision revision)
        {
            var cap_n = @namespace;

            while(true)
            {
                var obj = namespaces.AsQueryable()
                    .FirstOrDefault(w => w.Id == cap_n);
                if (obj == null)
                    obj = new Namespace { Id = cap_n };

                if (!obj.Revisions.Any(q => q.Id == revision.Id))
                {
                    obj.Revisions = obj.Revisions.Concat(new[] { revision }).ToArray();

                    var filter = Builders<Namespace>.Filter.Where(w => w.Id == cap_n);
                    var replaceResult = namespaces.ReplaceOne(filter, obj, new UpdateOptions { IsUpsert = true });
                }

                var name_split = cap_n.Split('.');
                if (name_split.Length == 1)
                    break;
                cap_n = string.Join(".", name_split.Take(name_split.Length - 1));
            }
        }

        public void DropHistory()
        {
            namespaces.DeleteMany(new BsonDocument());
        }
    }
}
