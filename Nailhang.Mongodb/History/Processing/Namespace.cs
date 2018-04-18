using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Nailhang.IndexBase.History.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Mongodb.History.Processing
{
    class Namespace
    {
        public string Id { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<string, ChangeInfo> ChangeInfos { get; set; } = new Dictionary<string, ChangeInfo>();
    }
}
