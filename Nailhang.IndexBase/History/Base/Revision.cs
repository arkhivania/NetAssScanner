using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.History.Base
{
    public class Revision
    {
        public int Id { get; set; }
        public string User { get; set; }
        public DateTime UtcDateTime { get; set; }
    }
}
