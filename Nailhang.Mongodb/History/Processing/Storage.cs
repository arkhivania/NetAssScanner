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
            
        }
    }
}
