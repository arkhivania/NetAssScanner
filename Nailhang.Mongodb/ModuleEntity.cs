using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb
{
    class ModuleEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        public string ModuleHeader { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
    }
}
