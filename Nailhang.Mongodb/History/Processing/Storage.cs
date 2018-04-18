using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Nailhang.IndexBase.History.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public IEnumerable<Change> GetChanges(string @namespace)
        {
            var nspace = namespaces.Find(q => q.Id == @namespace).FirstOrDefault();
            if (nspace != null)
                return nspace.ChangeInfos.Select(q =>
                {
                    return new Change
                    {
                        Revision = new Revision
                        {
                            Id = int.Parse(q.Key, CultureInfo.InvariantCulture),
                            User = q.Value.User,
                            UtcDateTime = q.Value.UtcDateTime
                        },
                        Modification = q.Value.Modification
                    };
                });
            return Enumerable.Empty<Change>();
        }

        public void StoreChangeToNamespace(string @namespace, Change change)
        {
            var cap_n = @namespace;

            while (true)
            {
                var obj = namespaces.AsQueryable()
                    .FirstOrDefault(w => w.Id == cap_n);
                if (obj == null)
                    obj = new Namespace { Id = cap_n };

                if (obj.ChangeInfos.TryGetValue(change.Revision.Id.ToString(CultureInfo.InvariantCulture), out ChangeInfo cinfo))
                {
                    if ((cinfo.Modification & change.Modification) != 0)
                        break;

                    cinfo.Modification |= change.Modification;
                }
                else
                {
                    obj.ChangeInfos.Add(change.Revision.Id.ToString(CultureInfo.InvariantCulture), new ChangeInfo
                    {
                        Modification = change.Modification,
                        User = change.Revision.User,
                        UtcDateTime = change.Revision.UtcDateTime
                    });
                }

                var filter = Builders<Namespace>.Filter.Where(w => w.Id == cap_n);
                var replaceResult = namespaces.ReplaceOne(filter, obj, new UpdateOptions { IsUpsert = true });

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
