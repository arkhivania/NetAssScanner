using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb.PublicStorage.Processing
{
    class AssemblyEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        public string StringId { get; set; }
        public string Data { get; set; }        
    }
}
