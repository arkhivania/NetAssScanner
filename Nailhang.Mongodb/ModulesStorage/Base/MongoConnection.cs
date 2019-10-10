using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb.ModulesStorage.Base
{
    public class MongoConnection
    {
        public string ConnectionString { get; set; } = "mongodb://localhost";
        public string DbName { get; set; } = "nailhang";
    }
}
