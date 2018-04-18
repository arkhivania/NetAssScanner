using Nailhang.IndexBase.History.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb.History.Processing
{
    public class ChangeInfo
    {
        public string User { get; set; }
        public DateTime UtcDateTime { get; set; }

        public Modification Modification { get; set; }
    }
}
