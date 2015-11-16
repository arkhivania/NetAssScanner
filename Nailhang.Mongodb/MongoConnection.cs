using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb
{
    public class MongoConnection : ConfigurationSection
    {
        [ConfigurationProperty(nameof(ConnectionString), DefaultValue = "mongodb://localhost", IsRequired = true)]
        public string ConnectionString
        {
            get
            {
                return (string)this[nameof(ConnectionString)];
            }
            set
            {
                this[nameof(ConnectionString)] = value;
            }
        }

        [ConfigurationProperty(nameof(DbName), DefaultValue = "nailhang", IsRequired = true)]
        public string DbName
        {
            get
            {
                return (string)this[nameof(DbName)];
            }
            set
            {
                this[nameof(DbName)] = value;
            }
        }

    }
}
