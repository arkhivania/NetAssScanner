using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb.ZonesStorage.Processing
{
    class ZoneEntity
    {
        [BsonId]
        public string Id { get; set; }

        public string Description { get; set; }
        public string[] ComponentIds { get; set; }
        public ZoneType Type { get; set; }
    }
}
